using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Đảm bảo không bị hủy khi load Scene mới
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Gọi hàm này để tạo hiệu ứng Hit-Stop
    public void HitStop(float duration, float timeScale)
    {
        StartCoroutine(HitStopCoroutine(duration, timeScale));
    }

    private IEnumerator HitStopCoroutine(float duration, float timeScale)
    {
        // Giảm tốc độ thời gian của game
        Time.timeScale = timeScale;
        
        // Chờ bằng thời gian THỰC (WaitForSecondsRealtime) để không bị ảnh hưởng bởi timeScale
        yield return new WaitForSecondsRealtime(duration);
        
        // Trả lại tốc độ bình thường
        Time.timeScale = 1f;
    }
}
