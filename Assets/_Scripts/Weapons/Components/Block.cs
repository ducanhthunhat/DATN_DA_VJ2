using System;
using DucAnh.CoreSystem;
using DucAnh.Utilities;
using DucAnh.Weapons.Modifiers;
using UnityEngine;

namespace DucAnh.Weapons.Components
{
    public class Block : WeaponComponent<BlockData, AttackBlock>
    {
        // Event fired off when an attack is blocked. The parameter is the GameObject of the entity that was blocked
        public event Action<GameObject> OnBlock;

        // The players DamageReceiver core component. We use the damage receiver's modifiers to modify the amount of damage successfully blocked attacks deal.
        private DamageReceiver damageReceiver;
        private KnockBackReceiver knockBackReceiver;
        private PoiseDamageReceiver poiseDamageReceiver;

        // The modifier that we give the DamageReceiver when the block window is active.
        private DamageModifier damageModifier;
        private BlockKnockBackModifier knockBackModifier;
        private BlockPoiseDamageModifier poiseDamageModifier;

        private CoreSystem.Movement movement;
        private ParticleManager particleManager;

        private bool isBlockWindowActive;
        private bool waitingForStart;
        private bool waitingForEnd;
        private float startTriggerTime;
        private float endTriggerTime;

        // Starts the block window by passing modifiers to receivers.
        private void StartBlockWindow()
        {
            isBlockWindowActive = true;

            damageModifier.OnModified += HandleModified;

            damageReceiver.Modifiers.AddModifier(damageModifier);
            knockBackReceiver.Modifiers.AddModifier(knockBackModifier);
            poiseDamageReceiver.Modifiers.AddModifier(poiseDamageModifier);
        }

        // Stops block window by removing modifiers from windows.
        private void StopBlockWindow()
        {
            isBlockWindowActive = false;

            damageModifier.OnModified -= HandleModified;

            damageReceiver.Modifiers.RemoveModifier(damageModifier);
            knockBackReceiver.Modifiers.RemoveModifier(knockBackModifier);
            poiseDamageReceiver.Modifiers.RemoveModifier(poiseDamageModifier);
        }

        protected override void HandleExit()
        {
            base.HandleExit();

            if (isBlockWindowActive)
            {
                StopBlockWindow();
            }
        }

        // Checks if source falls withing any blocked regions for the current attack. Also returns the block information
        private bool IsAttackBlocked(Transform source, out DirectionalInformation directionalInformation)
        {
            var angleOfAttacker = AngleUtilities.AngleFromFacingDirection(
                Core.Root.transform,
                source,
                movement.FacingDirection
            );

            return currentAttackData.IsBlocked(angleOfAttacker, out directionalInformation);
        }

        /*
         * The modifier is what tells us if a block was performed. It fires off an event when used. This handles that event and broadcasts
         * that information further
         */
        private void HandleModified(GameObject source)
        {
            particleManager.StartWithRandomRotation(currentAttackData.Particles, currentAttackData.ParticlesOffset);

            OnBlock?.Invoke(source);
        }

        private void HandleEnterAttackPhase(AttackPhases phase)
        {
            if (currentAttackData.BlockWindowStart.TryGetTriggerTime(phase, out var startTime))
            {
                startTriggerTime = startTime;
                waitingForStart = true;
            }
            if (currentAttackData.BlockWindowEnd.TryGetTriggerTime(phase, out var endTime))
            {
                endTriggerTime = endTime;
                waitingForEnd = true;
            }
        }

        #region Plumbing

        protected override void Start()
        {
            base.Start();

            movement = Core.GetCoreComponent<CoreSystem.Movement>();
            particleManager = Core.GetCoreComponent<ParticleManager>();

            knockBackReceiver = Core.GetCoreComponent<KnockBackReceiver>();
            damageReceiver = Core.GetCoreComponent<DamageReceiver>();
            poiseDamageReceiver = Core.GetCoreComponent<PoiseDamageReceiver>();

            // Create the modifier objects.
            damageModifier = new DamageModifier(IsAttackBlocked);
            knockBackModifier = new BlockKnockBackModifier(IsAttackBlocked);
            poiseDamageModifier = new BlockPoiseDamageModifier(IsAttackBlocked);

            AnimationEventHandler.OnEnterAttackPhase += HandleEnterAttackPhase;
        }

        private void Update()
        {
            if (waitingForStart && Time.time >= startTriggerTime)
            {
                waitingForStart = false;
                if (!isBlockWindowActive) StartBlockWindow();
            }

            if (waitingForEnd && Time.time >= endTriggerTime)
            {
                waitingForEnd = false;
                if (isBlockWindowActive) StopBlockWindow();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            AnimationEventHandler.OnEnterAttackPhase -= HandleEnterAttackPhase;
        }

        #endregion
    }
}
