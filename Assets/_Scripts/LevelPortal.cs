using DucAnh;
using UnityEngine;

public class LevelPortal : MonoBehaviour
{
    [Tooltip("Nếu true, chỉ Player mới được đi qua cổng")]
    [SerializeField] private bool onlyPlayer = true;
    
    [Tooltip("Tag của nhân vật chính")]
    [SerializeField] private string playerTag = "Player";

    private bool isPlayerInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (onlyPlayer && !collision.CompareTag(playerTag)) return;
        isPlayerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (onlyPlayer && !collision.CompareTag(playerTag)) return;
        isPlayerInRange = false;
    }

    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            // Khi người chơi đang đứng ở cổng và bấm phím F
            if (isPlayerInRange && UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame)
            {
                isPlayerInRange = false; // Ngăn chặn việc bấm liên tục
                Debug.Log("[LevelPortal] Người chơi bấm phím F! Đang chuyển qua Level tiếp theo...");
                GameManager.Instance.NextLevel();
                UIManager.Instance.OpenUI<LoadLVPanel>();
                UIManager.Instance.CloseUI<LoadLVPanel>(3f);
            }
        }
    }
}
