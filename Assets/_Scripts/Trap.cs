using UnityEngine;
using DucAnh.Combat.Damage;
using DucAnh.Combat.KnockBack;
using DucAnh.CoreSystem;
using System.Collections;

public class Trap : MonoBehaviour
{
    [Header("Cài đặt Bẫy")]
    [Tooltip("Lượng sát thương bẫy gây ra")]
    [SerializeField] private float damageAmount = 10f;
    
    [Tooltip("Lực nảy lên khi đạp trúng bẫy")]
    [SerializeField] private float knockbackStrength = 15f;
    
    [Tooltip("Góc nảy (X là đẩy ngang, Y là đẩy dọc)")]
    [SerializeField] private Vector2 knockbackAngle = new Vector2(0, 1); // Mặc định chỉ nảy thẳng đứng (Y=1)
    
    [Tooltip("Thời gian người chơi được bất tử (giây)")]
    [SerializeField] private float invincibilityDuration = 2f;

    // Biến nội bộ để theo dõi thời gian
    private float lastDamageTime = -100f;

    // Dùng OnTriggerStay2D để liên tục kiểm tra nếu người chơi vẫn đứng trên bẫy
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Kiểm tra xem đã hết thời gian hồi (2 giây) kể từ lần gây sát thương trước chưa
        if (Time.time >= lastDamageTime + invincibilityDuration)
        {
            // Tìm các Component nhận sát thương và knockback trên Player
            IDamageable damageable = collision.GetComponentInChildren<IDamageable>();
            IKnockBackable knockbackable = collision.GetComponentInChildren<IKnockBackable>();
            DamageReceiver damageReceiver = collision.GetComponentInChildren<DamageReceiver>();

            // Nếu vật thể chạm vào bẫy có máu (có thể là Player hoặc Enemy)
            if (damageable != null)
            {
                // 1. Tính toán hướng nảy (Nếu muốn nảy ngang thì dùng direction, nảy thẳng đứng thì direction không quan trọng)
                int direction = transform.position.x < collision.transform.position.x ? 1 : -1;

                // 2. Hất văng người chơi (Knockback)
                if (knockbackable != null)
                {
                    KnockBackData kbData = new KnockBackData(knockbackAngle, knockbackStrength, direction, this.gameObject);
                    knockbackable.KnockBack(kbData);
                }

                // 3. Trừ máu
                DamageData dmgData = new DamageData(damageAmount, this.gameObject);
                damageable.Damage(dmgData);

                // 4. Kích hoạt trạng thái Bất tử (Chỉ áp dụng nếu là Player có DamageReceiver)
                if (damageReceiver != null)
                {
                    StartCoroutine(InvincibilityRoutine(damageReceiver, invincibilityDuration));
                }

                // Cập nhật lại mốc thời gian vừa gây sát thương
                lastDamageTime = Time.time;
            }
        }
    }

    private IEnumerator InvincibilityRoutine(DamageReceiver receiver, float duration)
    {
        // Bật bất tử
        receiver.isInvincible = true;
        
        // Đợi 2 giây
        yield return new WaitForSeconds(duration);
        
        // Sau 2 giây, nếu component vẫn tồn tại thì tắt bất tử
        if (receiver != null)
        {
            receiver.isInvincible = false;
        }
    }
}
