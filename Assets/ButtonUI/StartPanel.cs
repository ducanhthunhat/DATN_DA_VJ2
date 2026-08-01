using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DucAnh
{
    public class StartPanel : UICanvas
    {
        void Start()
        {
            UIManager.Instance.PauseGame();
        }

        public void StartGame()
        {
            GameManager.Instance.StartGame(); // Gọi tới GameManager để sinh Map
            UIManager.Instance.ResumeGame();
            UIManager.Instance.CloseUI<StartPanel>(0.5f);
        }
    }
}
