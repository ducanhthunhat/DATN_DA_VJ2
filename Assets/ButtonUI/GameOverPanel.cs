using UnityEngine;
using System.Collections;

namespace DucAnh
{
    public class GameOverPanel : UICanvas
    {
        [Tooltip("Thời gian Panel hiển thị rõ dần (giây)")]
        [SerializeField] private float fadeDuration = 3f;

        public override void Open()
        {
            // Không gọi base.Open() vì base.Open() set alpha = 1 ngay lập tức.
            // Thay vào đó ta tự bật Gameobject và set alpha = 0.
            gameObject.SetActive(true);
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            StartCoroutine(FadeInAndRestart());
        }

        private IEnumerator FadeInAndRestart()
        {
            float elapsedTime = 0f;

            // Dùng unscaledDeltaTime để đề phòng trường hợp timeScale = 0 (game đang pause)
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 1f;

            // Sau khi hiện rõ xong 3 giây, gọi reset game về LV1
            global::GameManager.Instance.RestartGame();
            
            // Đóng Panel này lại
            CloseDirectly();
        }
    }
}
