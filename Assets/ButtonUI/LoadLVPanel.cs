using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DucAnh
{
    public class LoadLVPanel : UICanvas
    {
        [Tooltip("Kéo thả thanh Loading (UI Slider) vào đây")]
        [SerializeField] private Slider progressBar;

        [Tooltip("Thời gian mở mắt (giây)")]
        [SerializeField] private float fadeOutDuration = 1.5f;

        private RawImage vignetteImage;
        private Texture2D vignetteTexture;

        protected override void Awake()
        {
            base.Awake();
            SetupVignette();
        }

        private void SetupVignette()
        {
            // Tạo một GameObject con để vẽ màn đen
            GameObject vignetteObj = new GameObject("VignetteOverlay");
            vignetteObj.transform.SetParent(this.transform, false);
            
            // Đẩy Vignette lên trên cùng (che hết các BG khác)
            vignetteObj.transform.SetAsLastSibling(); 

            // Nếu có thanh Loading, đẩy thanh Loading lên TRÊN CÙNG để không bị màn đen che
            if (progressBar != null)
            {
                progressBar.transform.SetAsLastSibling();
            }

            vignetteImage = vignetteObj.AddComponent<RawImage>();
            vignetteImage.raycastTarget = false;
            vignetteImage.color = Color.black;
            
            // Kéo giãn phủ kín toàn màn hình
            vignetteImage.rectTransform.anchorMin = Vector2.zero;
            vignetteImage.rectTransform.anchorMax = Vector2.one;
            vignetteImage.rectTransform.offsetMin = Vector2.zero;
            vignetteImage.rectTransform.offsetMax = Vector2.zero;

            // Tạo texture hình lỗ hổng tròn
            int size = 256;
            vignetteTexture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            vignetteTexture.wrapMode = TextureWrapMode.Clamp; // Cực kỳ quan trọng để viền đen kéo dài vô tận
            
            Color[] pixels = new Color[size * size];
            Vector2 center = new Vector2(size / 2f, size / 2f);
            float maxRadius = size / 2f - 2f; // Giảm 2 pixel để chắc chắn 100% viền ngoài cùng là màu đục
            float minRadius = maxRadius * 0.5f; // Bán kính trong suốt

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    if (dist < minRadius)
                    {
                        pixels[y * size + x] = new Color(1, 1, 1, 0); // Trong suốt
                    }
                    else if (dist > maxRadius)
                    {
                        pixels[y * size + x] = Color.white; // Đục
                    }
                    else
                    {
                        // Gradient mờ ở viền
                        float alpha = (dist - minRadius) / (maxRadius - minRadius);
                        pixels[y * size + x] = new Color(1, 1, 1, alpha);
                    }
                }
            }
            vignetteTexture.SetPixels(pixels);
            vignetteTexture.Apply();

            vignetteImage.texture = vignetteTexture;
            
            // Tắt màu nền mặc định của Panel nếu có để không bị che mất lỗ hổng
            Image bg = GetComponent<Image>();
            if (bg != null) bg.enabled = false;
            
            SetVignetteZoom(50f); // Ban đầu mắt nhắm (lỗ hổng cực kỳ nhỏ)
        }

        private void SetVignetteZoom(float zoom)
        {
            if (vignetteImage == null) return;
            
            float aspect = (float)Screen.width / Screen.height;
            float w = zoom * aspect; // Bù trừ tỉ lệ màn hình để vòng tròn không bị méo thành oval
            float h = zoom;

            vignetteImage.uvRect = new Rect(0.5f - w / 2f, 0.5f - h / 2f, w, h);
        }

        public override void Open()
        {
            base.Open();
            
            // Bật lại TẤT CẢ giao diện đồ họa (hình nền Loading, Text, v.v...) của bạn
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            
            // Đẩy màn sương mù xuống DƯỚI CÙNG để nhường chỗ cho màn hình Loading tuyệt đẹp của bạn
            if (vignetteImage != null)
            {
                vignetteImage.transform.SetAsFirstSibling();
                SetVignetteZoom(50f); // Bắt đầu nhắm mắt (đã chuẩn bị sẵn ở lớp dưới cùng)
            }

            if (progressBar != null)
            {
                progressBar.gameObject.SetActive(true);
                progressBar.value = 0f;
                StartCoroutine(FillProgressBar(3f)); 
            }
        }

        private IEnumerator FillProgressBar(float duration)
        {
            float elapsed = 0f;
            float currentValue = 0f;
            float velocity = 0f; // Biến phụ trợ cho SmoothDamp

            // Vẫn giữ vài mốc dừng nhưng giảm thiểu lại để nó mượt hơn
            float stop1 = Random.Range(0.3f, 0.5f);
            float stop2 = Random.Range(0.7f, 0.9f);
            
            float[] stops = { stop1, stop2, 1f };
            int currentStop = 0;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;

                // Nếu chạy gần tới mốc dừng
                if (currentValue >= stops[currentStop] - 0.005f && currentStop < stops.Length - 1)
                {
                    // Nghỉ một nhịp rất ngắn (0.1 -> 0.2s) để không bị đơ quá lâu
                    float pause = Random.Range(0.1f, 0.2f);
                    yield return new WaitForSecondsRealtime(pause);
                    
                    elapsed += pause;
                    currentStop++;
                }
                else
                {
                    // Sử dụng SmoothDamp thay cho MoveTowards: 
                    // Thanh trượt sẽ tăng tốc từ từ và giảm tốc mượt mà khi gần tới đích
                    float smoothTime = Random.Range(0.15f, 0.3f); 
                    currentValue = Mathf.SmoothDamp(currentValue, stops[currentStop], ref velocity, smoothTime, 10f, Time.unscaledDeltaTime);
                }

                if (progressBar != null) progressBar.value = currentValue;

                yield return null;
            }
            
            if (progressBar != null) progressBar.value = 1f;
        }

        public override void Close(float time)
        {
            CancelInvoke();
            StartCoroutine(FadeOutCoroutine(time));
        }

        private IEnumerator FadeOutCoroutine(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);

            // GIAI ĐOẠN 2: Bắt đầu mở mắt
            // Tắt hết toàn bộ giao diện Loading của bạn (hình nền, thanh trượt...)
            foreach (Transform child in transform)
            {
                if (vignetteImage != null && child.gameObject != vignetteImage.gameObject)
                {
                    child.gameObject.SetActive(false);
                }
            }

            float elapsedTime = 0f;
            float startZoom = 50f;   // Kín mít (Mắt nhắm)
            float endZoom = 0.01f;   // Lỗ hổng khổng lồ (Mắt mở to)

            while (elapsedTime < fadeOutDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / fadeOutDuration;
                
                // Hàm lướt mượt: mở nhanh lúc đầu rồi chậm dần lúc sau
                float easedProgress = 1f - Mathf.Pow(1f - progress, 4f); 

                float currentZoom = Mathf.Lerp(startZoom, endZoom, easedProgress);
                SetVignetteZoom(currentZoom);
                
                yield return null;
            }

            CloseDirectly();
        }

        private void OnDestroy()
        {
            if (vignetteTexture != null) Destroy(vignetteTexture);
        }
    }
}
