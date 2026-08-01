using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        // Vẽ một hình tròn màu xanh lá cây trong Unity Editor để bạn dễ nhìn thấy điểm Spawn
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
