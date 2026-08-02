using UnityEngine;

namespace DucAnh.Weapons.Components
{
    public class HitStopOnHitBoxAction : WeaponComponent<HitStopOnHitBoxActionData, AttackHitStop>
    {
        private ActionHitBox hitBox;
        
        protected override void Start()
        {
            base.Start();

            // Tìm ActionHitBox nằm chung trên GameObject vũ khí
            hitBox = GetComponent<ActionHitBox>();
            
            if (hitBox != null)
            {
                hitBox.OnDetectedCollider2D += HandleDetectCollider2D;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (hitBox != null)
            {
                hitBox.OnDetectedCollider2D -= HandleDetectCollider2D;
            }
        }

        private void HandleDetectCollider2D(Collider2D[] colliders)
        {
            if (colliders.Length == 0) return;

            // Đọc thông số Hit Stop được bạn thiết lập trong ScriptableObject cho đòn đánh hiện tại
            if (currentAttackData != null && currentAttackData.Duration > 0)
            {
                if (TimeManager.Instance != null)
                {
                    TimeManager.Instance.HitStop(currentAttackData.Duration, currentAttackData.TimeScale);
                }
            }
        }
    }
}
