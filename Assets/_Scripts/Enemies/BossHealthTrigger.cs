using UnityEngine;
using DucAnh.CoreSystem;

public class BossHealthTrigger : MonoBehaviour
{
    [SerializeField] private Stats bossStats;
    [SerializeField] private string bossName = "Boss Tên Gì Đó";

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu đã hiện rồi thì không hiện lại nữa
        if (hasTriggered) return;

        // Chỉ hiện thanh máu khi người đi qua cửa là Player
        if (collision.CompareTag("Player"))
        {
            if (BossHealthBar.Instance != null)
            {
                // Vì script này giờ sẽ được gắn vào cái "Cửa" hoặc "Khu vực" của Boss,
                // Bạn bắt buộc phải kéo thả GameObject Boss vào ô bossStats trong Inspector nhé!
                if (bossStats != null)
                {
                    BossHealthBar.Instance.ShowBoss(bossStats, bossName);
                    hasTriggered = true; // Đánh dấu là đã bật thanh máu rồi
                }
                else
                {
                    Debug.LogError("BossHealthTrigger: Bạn chưa kéo thả Boss (hoặc component Stats của Boss) vào ô Boss Stats ở Inspector của cái Cửa này!");
                }
            }
        }
    }
}
