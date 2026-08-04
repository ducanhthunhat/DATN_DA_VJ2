using DucAnh.Combat.PoiseDamage;
using DucAnh.Interfaces;
using UnityEngine;

namespace DucAnh.Weapons.Components
{
    public class PoiseDamage : WeaponComponent<PoiseDamageData, AttackPoiseDamage>
    {
        private ActionHitBox hitBox;

        private void HandleDetectCollider2D(Collider2D[] colliders)
        {
            System.Collections.Generic.List<IPoiseDamageable> damagedTargets = new System.Collections.Generic.List<IPoiseDamageable>();

            foreach (var item in colliders)
            {
                if (item.TryGetComponent(out IPoiseDamageable poiseDamageable))
                {
                    if (!damagedTargets.Contains(poiseDamageable))
                    {
                        poiseDamageable.DamagePoise(new Combat.PoiseDamage.PoiseDamageData(currentAttackData.Amount, Core.Root));
                        damagedTargets.Add(poiseDamageable);
                    }
                }
            }
        }
        
        protected override void Start()
        {
            base.Start();

            hitBox = GetComponent<ActionHitBox>();

            hitBox.OnDetectedCollider2D += HandleDetectCollider2D;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            hitBox.OnDetectedCollider2D -= HandleDetectCollider2D;
        }
    }
}
