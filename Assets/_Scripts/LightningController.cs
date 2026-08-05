using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LightningController : MonoBehaviour
{
    [Header("Cài đặt Sấm chớp")]
    [Tooltip("Kéo thả tấm ảnh trắng (Image) dùng để nháy sáng vào đây")]
    [SerializeField] private Image flashImage;
    
    [Tooltip("Thời gian chờ ít nhất giữa các đợt sấm")]
    [SerializeField] private float minTimeBetweenFlashes = 5f;
    
    [Tooltip("Thời gian chờ lâu nhất giữa các đợt sấm")]
    [SerializeField] private float maxTimeBetweenFlashes = 15f;

    private void Start()
    {
        if (flashImage != null)
        {
            // Đảm bảo ban đầu màn hình không bị sáng
            Color c = flashImage.color;
            c.a = 0f;
            flashImage.color = c;
            
            // Bắt đầu vòng lặp tạo sấm chớp
            StartCoroutine(LightningRoutine());
        }
    }

    private IEnumerator LightningRoutine()
    {
        while (true)
        {
            // Đợi một khoảng thời gian ngẫu nhiên
            yield return new WaitForSeconds(Random.Range(minTimeBetweenFlashes, maxTimeBetweenFlashes));

            // Mỗi đợt sấm sẽ giật liên tục từ 1 đến 3 nhịp (như sấm ngoài đời)
            int flashCount = Random.Range(1, 4);
            for (int i = 0; i < flashCount; i++)
            {
                yield return StartCoroutine(Flash());
                yield return new WaitForSeconds(Random.Range(0.05f, 0.15f)); // Thời gian tối đen giữa các nhịp giật
            }
        }
    }

    private IEnumerator Flash()
    {
        Color c = flashImage.color;
        
        // 1. Chớp sáng đột ngột lên (Random độ sáng từ 40% đến 80% để mỗi cú chớp một khác)
        c.a = Random.Range(0.4f, 0.8f); 
        flashImage.color = c;

        // 2. Mờ dần đi rất nhanh
        while (flashImage.color.a > 0)
        {
            // Chỉnh số 4f to hơn thì chớp tắt càng nhanh, nhỏ hơn thì mờ từ từ
            c.a -= Time.deltaTime * 4f; 
            flashImage.color = c;
            yield return null;
        }
        
        // Đảm bảo tắt hẳn
        c.a = 0f;
        flashImage.color = c;
    }
}
