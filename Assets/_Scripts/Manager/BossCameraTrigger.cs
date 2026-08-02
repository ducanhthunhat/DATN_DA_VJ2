using System.Collections;
using UnityEngine;
using Cinemachine;

public class BossCameraTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform bossTransform;
    [SerializeField] private float lookDuration = 2f;
    [SerializeField] private string playerTag = "Player";

    [SerializeField] private GameObject doorBlocker; // Tường/cửa dùng để chặn đường

    [Header("Door Visuals")]
    [SerializeField] private SpriteRenderer doorSpriteRenderer; // Nơi hiển thị hình ảnh cánh cửa
    [SerializeField] private Sprite closedDoorSprite; // Hình ảnh cánh cửa lúc đóng

    [Header("Cinematic Controls")]
    [SerializeField] private Entity bossScript; // Kéo Boss vào đây (tự nhận Script Entity/Boss1)

    private CinemachineVirtualCamera vcam;
    private bool hasTriggered = false;
    private UnityEngine.InputSystem.PlayerInput playerInput;

    private void Awake()
    {
        // Tự động tìm Camera ảo trong Scene
        vcam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        
        // Đảm bảo cửa chặn được tắt lúc ban đầu
        if (doorBlocker != null)
        {
            doorBlocker.SetActive(false);
        }

        // Đóng băng Boss lúc mới vào game (để Boss đứng im chờ)
        if (bossScript != null)
        {
            bossScript.enabled = false;
            
            // Ép Animator chuyển sang trạng thái Idle (đứng thở) thông qua biến Bool
            Animator bossAnim = bossScript.GetComponent<Animator>();
            if (bossAnim != null)
            {
                bossAnim.SetBool("idle", true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu người chơi chạm vào vùng trigger và chưa kích hoạt lần nào
        if (!hasTriggered && collision.CompareTag(playerTag))
        {
            hasTriggered = true;
            
            // Lấy component Input của người chơi để lát nữa khóa lại
            playerInput = collision.GetComponent<UnityEngine.InputSystem.PlayerInput>();

            // Bật bức tường chặn đường quay ra
            if (doorBlocker != null)
            {
                doorBlocker.SetActive(true);
            }

            // Đổi hình ảnh cánh cửa từ mở sang đóng
            if (doorSpriteRenderer != null && closedDoorSprite != null)
            {
                doorSpriteRenderer.sprite = closedDoorSprite;
            }

            StartCoroutine(FocusBossCoroutine());
        }
    }

    private IEnumerator FocusBossCoroutine()
    {
        if (vcam != null && bossTransform != null)
        {
            // Khóa bàn phím người chơi (đứng im)
            if (playerInput != null)
            {
                playerInput.DeactivateInput();
                
                // Đồng thời set velocity của Rigidbody2D về 0 để không bị trượt đi
                var rb = playerInput.GetComponent<Rigidbody2D>();
                if (rb != null) rb.velocity = Vector2.zero;
            }

            // Lưu lại mục tiêu hiện tại (Player)
            Transform originalTarget = vcam.Follow;
            
            // Chuyển Camera sang nhìn Boss
            vcam.Follow = bossTransform;

            // Đợi 2 giây
            yield return new WaitForSeconds(lookDuration);

            // Trả Camera về lại cho Player
            vcam.Follow = originalTarget;

            // Mở khóa bàn phím người chơi
            if (playerInput != null)
            {
                playerInput.ActivateInput();
            }

            // Tắt ép buộc animation idle trước khi đánh thức Boss
            if (bossScript != null)
            {
                Animator bossAnim = bossScript.GetComponent<Animator>();
                if (bossAnim != null)
                {
                    bossAnim.SetBool("idle", false);
                }
                
                // Đánh thức Boss dậy!
                bossScript.enabled = true;
            }
        }
        else
        {
            Debug.LogWarning("BossCameraTrigger: Không tìm thấy Boss hoặc Camera!");
        }
    }
}
