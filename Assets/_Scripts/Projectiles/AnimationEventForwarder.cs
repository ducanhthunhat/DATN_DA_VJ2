using UnityEngine;
using DucAnh.ObjectPoolSystem;

namespace DucAnh.Projectiles
{
    // Script này dùng để gắn vào cục con (Visual) chứa Animator
    // Nhiệm vụ của nó là nhận tín hiệu Animation Event từ Animator 
    // và báo lên cho thằng cha (chứa ObjectPoolItem) để xóa object.
    public class AnimationEventForwarder : MonoBehaviour
    {
        public void ReturnItem()
        {
            // Tìm ObjectPoolItem ở thằng cha
            ObjectPoolItem poolItem = GetComponentInParent<ObjectPoolItem>();
            
            if (poolItem != null)
            {
                poolItem.ReturnItem(); // Báo cho cha thu về pool
            }
            else
            {
                // Nếu cha không có Pool thì xóa luôn cha
                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
