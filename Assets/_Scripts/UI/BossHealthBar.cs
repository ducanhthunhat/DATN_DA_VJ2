using UnityEngine;
using UnityEngine.UI;
using DucAnh.CoreSystem;

public class BossHealthBar : MonoBehaviour
{
    public static BossHealthBar Instance { get; private set; }

    [SerializeField] private Slider hpSlider;
    [SerializeField] private GameObject healthBarContainer; // Kéo thả cái GameObject to nhất chứa thanh HP vào đây
    // [SerializeField] private Text bossNameText; // Mở comment dòng này nếu bạn có Text tên Boss
    
    private Stats currentBossStats;

    private void Awake()
    {
        // Singleton pattern để Boss ở bất kỳ đâu cũng có thể dễ dàng gọi thanh máu này lên
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Mặc định ẩn thanh máu đi khi mới vào game
        if (healthBarContainer != null)
        {
            healthBarContainer.SetActive(false);
        }
    }

    // Hàm này sẽ được Boss gọi khi nó thức tỉnh/xuất hiện
    public void ShowBoss(Stats bossStats, string bossName = "")
    {
        currentBossStats = bossStats;

        if (healthBarContainer != null)
        {
            healthBarContainer.SetActive(true);
        }

        /* 
        if (bossNameText != null) {
            bossNameText.text = bossName;
        }
        */

        if (hpSlider == null)
        {
            hpSlider = GetComponentInChildren<Slider>();
        }

        // Cập nhật máu lần đầu
        UpdateHealthBar(currentBossStats.Health.CurrentValue, currentBossStats.Health.MaxValue);

        // Lắng nghe sự thay đổi máu
        currentBossStats.Health.OnCurrentValueChanged += UpdateHealthBar;
        
        // Lắng nghe lúc Boss chết (máu = 0) để ẩn thanh HP đi
        currentBossStats.Health.OnCurrentValueZero += HideBossHealthBar;
    }

    private void HideBossHealthBar()
    {
        if (healthBarContainer != null)
        {
            healthBarContainer.SetActive(false);
        }

        if (currentBossStats != null)
        {
            currentBossStats.Health.OnCurrentValueChanged -= UpdateHealthBar;
            currentBossStats.Health.OnCurrentValueZero -= HideBossHealthBar;
        }
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHealth;
            hpSlider.value = currentHealth;
        }
    }
}
