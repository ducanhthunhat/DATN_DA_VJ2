using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DucAnh
{
    public class PausePanel : UICanvas
    {
        public void ResumeGame()
        {
            UIManager.Instance.CloseUI<PausePanel>(0.2f);
            UIManager.Instance.ResumeGame();
        }

        public void OpenOptions()
        {
            UIManager.Instance.OpenUI<OptionsPanel>();
        }

        public void QuitToMenu()
        {
            // Có 2 cách: Load lại Scene hoặc mở StartPanel.
            // Vì Game đang dùng chung 1 Scene (Main) nên ta sẽ reset logic về StartPanel
            
            UIManager.Instance.ResumeGame(); // Phải resume để timeScale chạy lại
            
            // Xóa Save cũ nếu muốn reset từ đầu, hoặc không xóa nếu muốn Continue
            // Tùy theo logic của bạn. Ở đây chỉ quay về StartPanel.
            
            UIManager.Instance.CloseAll(); // Đóng tất cả
            
            // Dọn dẹp Player khỏi map (chết ảo)
            Player player = FindObjectOfType<Player>(true);
            if (player != null)
            {
                player.gameObject.SetActive(false); // Ẩn Player đi để load lại sau
            }

            UIManager.Instance.OpenUI<StartPanel>();
        }
    }
}
