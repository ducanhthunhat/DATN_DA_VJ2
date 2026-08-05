using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Tooltip("Tốc độ xoay (độ/giây). Số dương xoay ngược kim đồng hồ, số âm xoay cùng chiều.")]
    public float rotationSpeed = -180f;

    private void Update()
    {
        // Xoay object quanh trục Z
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
