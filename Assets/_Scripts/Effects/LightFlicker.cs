using UnityEngine;

namespace DucAnh.Effects
{
    public class LightFlicker : MonoBehaviour
    {
        [Header("Settings")]
        public float flickerSpeed = 10f;
        public float flickerSize = 0.1f;

        private Vector3 baseScale;

        void Start()
        {
            baseScale = transform.localScale;
        }

        void Update()
        {
            // Làm vòng sáng to nhỏ liên tục ngẫu nhiên
            float randomFlicker = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f) * flickerSize;
            transform.localScale = baseScale + new Vector3(randomFlicker, randomFlicker, 0f);
        }
    }
}
