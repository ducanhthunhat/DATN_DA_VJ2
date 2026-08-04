using UnityEngine;
using UnityEngine.UI;
using DucAnh.CoreSystem;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    
    // Thay vì tham chiếu trực tiếp đến Player, chúng ta có thể tham chiếu trực tiếp tới Core component "Stats" của Player.
    [SerializeField] private Stats playerStats;

    private void Start()
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerHealthBar: Chưa kéo Player Stats vào Inspector!");
            return;
        }

        if (hpSlider == null)
        {
            hpSlider = GetComponent<Slider>();
        }

        // Khởi tạo thanh máu dựa trên máu tối đa ban đầu
        UpdateHealthBar(playerStats.Health.CurrentValue, playerStats.Health.MaxValue);

        // Lắng nghe sự kiện máu thay đổi
        playerStats.Health.OnCurrentValueChanged += UpdateHealthBar;
    }

    private void OnDestroy()
    {
        // Gỡ lắng nghe khi UI bị phá hủy để tránh lỗi memory leak
        if (playerStats != null)
        {
            playerStats.Health.OnCurrentValueChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (hpSlider != null)
        {
            // Thiết lập giá trị Slider
            hpSlider.maxValue = maxHealth;
            hpSlider.value = currentHealth;
        }
    }
}
