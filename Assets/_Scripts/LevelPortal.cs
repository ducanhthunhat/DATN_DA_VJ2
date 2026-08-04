using DucAnh;
using UnityEngine;

public class LevelPortal : MonoBehaviour
{
    [Tooltip("Nếu true, chỉ Player mới được đi qua cổng")]
    [SerializeField] private bool onlyPlayer = true;
    
    [Tooltip("Tag của nhân vật chính")]
    [SerializeField] private string playerTag = "Player";

    [Header("Animation (Tùy chọn)")]
    [Tooltip("Kéo thả Animator của cánh cửa vào đây")]
    [SerializeField] private Animator doorAnimator;
    [Tooltip("Tên biến Bool trong Animator để mở cửa (VD: isNear)")]
    [SerializeField] private string isNearBoolName = "isNear";

    private bool isPlayerInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (onlyPlayer && !collision.CompareTag(playerTag)) return;
        isPlayerInRange = true;
        
        if (doorAnimator != null && !string.IsNullOrEmpty(isNearBoolName))
        {
            doorAnimator.SetBool(isNearBoolName, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (onlyPlayer && !collision.CompareTag(playerTag)) return;
        isPlayerInRange = false;

        if (doorAnimator != null && !string.IsNullOrEmpty(isNearBoolName))
        {
            doorAnimator.SetBool(isNearBoolName, false);
        }
    }

    private void Update()
    {
        // Xử lý phím F để qua màn
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
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
