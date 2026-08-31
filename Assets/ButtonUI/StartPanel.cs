using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DucAnh
{
    public class StartPanel : UICanvas
    {
        [Tooltip("Text hiển thị lỗi khi không có dữ liệu (có thể bỏ trống)")]
        [SerializeField] private TMPro.TextMeshProUGUI messageText;
        [Tooltip("Khoảng cách bay lên của chữ cảnh báo")]
        [SerializeField] private float floatDistance = 50f;
        [Tooltip("Thời gian mờ dần (giây)")]
        [SerializeField] private float fadeDuration = 2f;

        private Vector2 originalPos;
        private Color originalColor;
        private Coroutine warningCoroutine;
        private bool hasInitialized = false;

        void Start()
        {
            if (messageText != null)
            {
                originalPos = messageText.rectTransform.anchoredPosition;
                originalColor = messageText.color;
                hasInitialized = true;
                
                // Tắt Raycast Target để chữ không chặn click chuột của người chơi
                messageText.raycastTarget = false;
            }
            UIManager.Instance.PauseGame();
        }

        public void StartGame()
        {
            GameManager.Instance.StartGame(); // Gọi tới GameManager để sinh Map
            UIManager.Instance.ResumeGame();
            
            // Hiển thị màn hình Load Level
            UIManager.Instance.OpenUI<LoadLVPanel>();
            UIManager.Instance.CloseUI<LoadLVPanel>(3f);
            
            UIManager.Instance.CloseUI<StartPanel>(0.5f);
        }

        public void ContinueGame()
        {
            // Bỏ chọn nút hiện tại để có thể Hover lại bình thường
            if (UnityEngine.EventSystems.EventSystem.current != null)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            }

            if (GameManager.Instance.HasSaveData())
            {
                GameManager.Instance.ContinueGame();
                UIManager.Instance.ResumeGame();
                
                // Hiển thị màn hình Load Level
                UIManager.Instance.OpenUI<LoadLVPanel>();
                UIManager.Instance.CloseUI<LoadLVPanel>(3f);

                UIManager.Instance.CloseUI<StartPanel>(0.5f);
            }
            else
            {
                if (messageText != null)
                {
                    if (warningCoroutine != null) StopCoroutine(warningCoroutine);
                    warningCoroutine = StartCoroutine(ShowAndFadeWarning());
                }
                else
                {
                    Debug.LogWarning("[StartPanel] KHÔNG CÓ DỮ LIỆU. (Hãy kéo Component TextMeshProUGUI vào biến messageText để hiển thị lỗi).");
                }
            }
        }

        private IEnumerator ShowAndFadeWarning()
        {
            if (!hasInitialized) yield break;

            // Đặt lại vị trí và màu sắc ban đầu trước khi bắt đầu hiệu ứng
            messageText.rectTransform.anchoredPosition = originalPos;
            messageText.color = originalColor;
            messageText.text = "Không có dữ liệu!";
            messageText.gameObject.SetActive(true); 
            
            float elapsedTime = 0f;
            
            // Chạy vòng lặp từ từ di chuyển lên và giảm Alpha (sử dụng unscaledDeltaTime vì Time.timeScale đang = 0)
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / fadeDuration;
                
                // Lerp vị trí bay lên
                messageText.rectTransform.anchoredPosition = originalPos + Vector2.up * (floatDistance * progress);
                
                // Lerp độ mờ Alpha về 0
                Color newColor = originalColor;
                newColor.a = Mathf.Lerp(originalColor.a, 0f, progress);
                messageText.color = newColor;

                yield return null;
            }

            // Tắt đi khi mờ hẳn
            messageText.gameObject.SetActive(false);
        }

        public void OpenOptions()
        {
            UIManager.Instance.OpenUI<OptionsPanel>();
        }

        public void QuitGame()
        {
            Debug.Log("Đang thoát game (Lệnh này sẽ có tác dụng khi Build file .exe hoặc .apk)");
            Application.Quit();
        }
    }
}
