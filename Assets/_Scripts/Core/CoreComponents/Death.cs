using UnityEngine;

namespace DucAnh.CoreSystem
{
    public class Death : CoreComponent
    {
        [SerializeField] private GameObject[] deathParticles;

        private ParticleManager ParticleManager =>
            particleManager ? particleManager : core.GetCoreComponent(ref particleManager);
    
        private ParticleManager particleManager;

        private Stats Stats => stats ? stats : core.GetCoreComponent(ref stats);
        private Stats stats;
    
        public void Die()
        {
            foreach (var particle in deathParticles)
            {
                ParticleManager.StartParticles(particle);
            }
        
            core.transform.parent.gameObject.SetActive(false);

            // Kiểm tra xem đối tượng vừa chết có phải là Player không
            if (core.transform.parent.CompareTag("Player"))
            {
                // Thay vì gọi Restart ngay, ta bật bảng GameOver (sau 3s nó sẽ tự gọi Restart)
                global::UIManager.Instance.OpenUI<DucAnh.GameOverPanel>();
            }
        }

        private void OnEnable()
        {
            Stats.Health.OnCurrentValueZero += Die;
        }

        private void OnDisable()
        {
            Stats.Health.OnCurrentValueZero -= Die;
        }
    }
}
