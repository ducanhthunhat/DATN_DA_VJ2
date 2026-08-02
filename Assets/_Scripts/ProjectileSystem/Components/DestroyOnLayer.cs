using DucAnh.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace DucAnh.ProjectileSystem.Components
{
    public class DestroyOnLayer : ProjectileComponent
    {
        public UnityEvent<RaycastHit2D> OnHit;

        [field: SerializeField] public LayerMask LayerMask { get; private set; }

        private HitBox hitBox;

        protected override void Awake()
        {
            base.Awake();

            hitBox = GetComponent<HitBox>();
            hitBox.OnRaycastHit2D.AddListener(HandleRaycastHit2D);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            hitBox.OnRaycastHit2D.RemoveListener(HandleRaycastHit2D);
        }

        private void HandleRaycastHit2D(RaycastHit2D[] hits)
        {
            if (!Active)
                return;

            foreach (var hit in hits)
            {
                // Kiểm tra xem vật thể va chạm có nằm trong LayerMask chỉ định không
                if (LayerMaskUtilities.IsLayerInMask(hit, LayerMask))
                {
                    // Phát event để spawn effect (Particle)
                    OnHit?.Invoke(hit);

                    // Hủy GameObject của viên đạn (hoặc đưa về Object Pool nếu có)
                    var poolItem = projectile.GetComponent<DucAnh.ObjectPoolSystem.ObjectPoolItem>();
                    if (poolItem != null)
                    {
                        poolItem.ReturnItem();
                    }
                    else
                    {
                        Destroy(projectile.gameObject);
                    }
                    return;
                }
            }
        }
    }
}
