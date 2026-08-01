using UnityEngine;
using DucAnh.Combat.Damage;
using DucAnh.Combat.KnockBack;

namespace DucAnh.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        private float speed;
        private float travelDistance;
        private float xStartPos;
        private float damageAmount;

        [SerializeField]
        private float gravity;
        [SerializeField]
        private float damageRadius;
        [SerializeField]
        private Vector2 knockBackAngle = new Vector2(1, 1);
        [SerializeField]
        private float knockBackStrength = 10f;
        [SerializeField]
        private bool destroyOnHitGround = true; // Bật = Biến mất khi chạm đất, Tắt = Cắm chặt vào đất
        [SerializeField]
        private bool destroyOnHitPlayer = true; // Bật = Biến mất khi chạm Player, Tắt = Xuyên qua Player

        private Rigidbody2D rb;

        private bool isGravityOn;
        private bool hasHitGround;

        [SerializeField]
        private LayerMask whatIsGround;
        [SerializeField]
        private LayerMask whatIsPlayer;
        [SerializeField]
        private Transform damagePosition;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            rb.gravityScale = 0.0f;
            rb.velocity = transform.right * speed;

            isGravityOn = false;

            xStartPos = transform.position.x;
        }

        private void Update()
        {
            if (!hasHitGround)
            {
                if (isGravityOn)
                {
                    float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
            }
        }

        private void FixedUpdate()
        {
            if (!hasHitGround)
            {
                Collider2D damageHit = Physics2D.OverlapCircle(damagePosition.position, damageRadius, whatIsPlayer);
                Collider2D groundHit = Physics2D.OverlapCircle(damagePosition.position, damageRadius, whatIsGround);

                if (damageHit)
                {
                    IDamageable damageable = damageHit.GetComponentInChildren<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.Damage(new DamageData(damageAmount, gameObject));
                    }

                    IKnockBackable knockBackable = damageHit.GetComponentInChildren<IKnockBackable>();
                    if (knockBackable != null)
                    {
                        int direction = rb.velocity.x > 0 ? 1 : -1;
                        knockBackable.KnockBack(new KnockBackData(knockBackAngle, knockBackStrength, direction, gameObject));
                    }

                    if (destroyOnHitPlayer)
                    {
                        DucAnh.ObjectPoolSystem.ObjectPoolItem poolItem = GetComponent<DucAnh.ObjectPoolSystem.ObjectPoolItem>();
                        if (poolItem != null)
                        {
                            poolItem.ReturnItem();
                        }
                        else
                        {
                            Destroy(gameObject);
                        }
                    }
                }

                if (groundHit)
                {
                    hasHitGround = true;
                    rb.gravityScale = 0f;
                    rb.velocity = Vector2.zero;

                    if (destroyOnHitGround)
                    {
                        // Trả đạn về Pool hoặc Hủy (Destroy) đạn
                        DucAnh.ObjectPoolSystem.ObjectPoolItem poolItem = GetComponent<DucAnh.ObjectPoolSystem.ObjectPoolItem>();
                        if (poolItem != null)
                        {
                            poolItem.ReturnItem(); // Trả về pool nếu có dùng Object Pool
                        }
                        else
                        {
                            Destroy(gameObject); // Xóa thẳng nếu không dùng Pool
                        }
                    }
                }

                if (Mathf.Abs(xStartPos - transform.position.x) >= travelDistance && !isGravityOn)
                {
                    isGravityOn = true;
                    rb.gravityScale = gravity;
                }
            }        
        }

        public void FireProjectile(float speed, float travelDistance, float damage)
        {
            this.speed = speed;
            this.travelDistance = travelDistance;
            this.damageAmount = damage;
        }

        private void OnDrawGizmos()
        {
            if (damagePosition != null)
            {
                Gizmos.DrawWireSphere(damagePosition.position, damageRadius);
            }
        }
    }
}

