using System;
using DucAnh.CoreSystem;
using DucAnh.Utilities;
using DucAnh.Weapons.Modifiers;
using UnityEngine;
using static DucAnh.Combat.Parry.CombatParryUtilities;

namespace DucAnh.Weapons.Components
{
    /*
     * Parry works essentially the same as the Block weapon component. It passes modifiers to the various
     * player -Receiver Core Components while the parry window is active. If the damage modifier is triggered
     * it counts as a successful parry and the entity that tried to do damage is parried.
     */
    public class Parry : WeaponComponent<ParryData, AttackParry>
    {
        public event Action<GameObject> OnParry;

        private DamageReceiver damageReceiver;
        private KnockBackReceiver knockBackReceiver;
        private PoiseDamageReceiver poiseDamageReceiver;

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

        private void StartParryWindow()
        {
            isBlockWindowActive = true;

            damageModifier.OnModified += HandleParry;


            damageReceiver.Modifiers.AddModifier(damageModifier);
            knockBackReceiver.Modifiers.AddModifier(knockBackModifier);
            poiseDamageReceiver.Modifiers.AddModifier(poiseDamageModifier);
        }

        private void StopParryWindow()
        {
            isBlockWindowActive = false;

            damageModifier.OnModified -= HandleParry;

            damageReceiver.Modifiers.RemoveModifier(damageModifier);
            knockBackReceiver.Modifiers.RemoveModifier(knockBackModifier);
            poiseDamageReceiver.Modifiers.RemoveModifier(poiseDamageModifier);
        }

        protected override void HandleExit()
        {
            base.HandleExit();

            damageReceiver.Modifiers.RemoveModifier(damageModifier);
            knockBackReceiver.Modifiers.RemoveModifier(knockBackModifier);
            poiseDamageReceiver.Modifiers.RemoveModifier(poiseDamageModifier);
        }

        private bool IsAttackParried(Transform source, out DirectionalInformation directionalInformation)
        {
            var angleOfAttacker = AngleUtilities.AngleFromFacingDirection(
                Core.Root.transform,
                source,
                movement.FacingDirection
            );

            return currentAttackData.IsBlocked(angleOfAttacker, out directionalInformation);
        }

        private void HandleParry(GameObject parriedGameObject)
        {
            /*
             * The modifier is only used to detect an enemy making contact with the player from allowed directions.
             * If that happens we trigger the parry effects.
             */
            weapon.Anim.SetTrigger("parry");

            OnParry?.Invoke(parriedGameObject);

            particleManager.StartWithRandomRotation(currentAttackData.Particles, currentAttackData.ParticlesOffset);

            /*
             * Inform the entity that it has been parried (if it implements IParryable).
             */
            TryParry(parriedGameObject, new Combat.Parry.ParryData(Core.Root), out _, out _);
        }

        private void HandleEnterAttackPhase(AttackPhases phase)
        {
            if (currentAttackData.ParryWindowStart.TryGetTriggerTime(phase, out var startTime))
            {
                startTriggerTime = startTime;
                waitingForStart = true;
            }
            if (currentAttackData.ParryWindowEnd.TryGetTriggerTime(phase, out var endTime))
            {
                endTriggerTime = endTime;
                waitingForEnd = true;
            }
        }

        #region Plumbing

        protected override void Start()
        {
            base.Start();

            damageReceiver = Core.GetCoreComponent<DamageReceiver>();
            knockBackReceiver = Core.GetCoreComponent<KnockBackReceiver>();
            poiseDamageReceiver = Core.GetCoreComponent<PoiseDamageReceiver>();

            movement = Core.GetCoreComponent<CoreSystem.Movement>();
            particleManager = Core.GetCoreComponent<ParticleManager>();

            damageModifier = new DamageModifier(IsAttackParried);
            knockBackModifier = new BlockKnockBackModifier(IsAttackParried);
            poiseDamageModifier = new BlockPoiseDamageModifier(IsAttackParried);

            AnimationEventHandler.OnEnterAttackPhase += HandleEnterAttackPhase;
        }

        private void Update()
        {
            if (waitingForStart && Time.time >= startTriggerTime)
            {
                waitingForStart = false;
                if (!isBlockWindowActive) StartParryWindow();
            }

            if (waitingForEnd && Time.time >= endTriggerTime)
            {
                waitingForEnd = false;
                if (isBlockWindowActive) StopParryWindow();
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
