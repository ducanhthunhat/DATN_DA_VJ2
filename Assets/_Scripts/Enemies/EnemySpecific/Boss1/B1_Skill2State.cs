using UnityEngine;
using DucAnh.Projectiles;

public class B1_Skill2State : RangedAttackState
{
    private Boss1 boss;

    public B1_Skill2State(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_RangedAttackState stateData, Boss1 boss) 
        : base(entity, stateMachine, animBoolName, attackPosition, stateData)
    {
        this.boss = boss;
    }

    public override void Enter()
    {
        base.Enter();
        boss.lastSkillTime = Time.time; // Cập nhật đồng hồ chung
        boss.lastSkillUsed = 2; // Đánh dấu đã dùng Skill 2
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            stateMachine.ChangeState(boss.idleState);
        }
    }

    public override void TriggerAttack()
    {
        // Xóa dòng base.TriggerAttack() đi để không bị sinh ra 1 viên đạn mặc định
        
        int projectileCount = 8; // Bắn ra 8 hướng (có thể tăng lên 12 hoặc 16)
        float angleStep = 360f / projectileCount;
        float currentAngle = 0f;

        for (int i = 0; i < projectileCount; i++)
        {
            // Tính toán góc xoay
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            
            // Tự động kéo điểm đẻ đạn tụt xuống một chút nếu Boss đang ở Phase 2 (vì cơ thể bị phóng to 1.15x)
            Vector3 spawnPos = attackPosition.position;
            if (boss.IsPhase2()) 
            {
                spawnPos.y -= 0.5f; 
            }

            // Tạo viên đạn
            GameObject newProjectile = GameObject.Instantiate(stateData.projectile, spawnPos, rotation);
            
            // Kích hoạt viên đạn bay đi
            Projectile projectileScript = newProjectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                float damage = stateData.projectileDamage;
                if (boss.IsPhase2()) damage *= boss.phase2DamageMultiplier;

                projectileScript.FireProjectile(stateData.projectileSpeed, stateData.projectileTravelDistance, damage);
            }
            
            currentAngle += angleStep;
        }
    }
}
