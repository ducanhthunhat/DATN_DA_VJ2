using UnityEngine;

public class B1_Skill1State : AttackState
{
    private Boss1 boss;
    protected D_RangedAttackState stateData;

    public B1_Skill1State(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_RangedAttackState stateData, Boss1 boss) 
        : base(entity, stateMachine, animBoolName, attackPosition)
    {
        this.boss = boss;
        this.stateData = stateData;
    }

    public override void Enter()
    {
        base.Enter();
        boss.lastSkillTime = Time.time; // Cập nhật đồng hồ chung
        boss.lastSkillUsed = 1; // Đánh dấu đã dùng Skill 1
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            stateMachine.ChangeState(boss.idleState);
        }
    }

    // Hàm này sẽ chạy khi Animation Event "TriggerAttack" được kích hoạt
    public override void TriggerAttack()
    {
        base.TriggerAttack();
        
        // Sinh ra Prefab đứng yên tại vị trí chỉ định từ Data
        if (stateData != null && stateData.projectile != null && attackPosition != null)
        {
            // Nếu có đủ hết thì đẻ ra
            GameObject newSkill = GameObject.Instantiate(stateData.projectile, attackPosition.position, attackPosition.rotation);
            
            // Kích hoạt sát thương và TỐC ĐỘ BAY cho các bẫy/đạn
            DucAnh.Projectiles.Projectile proj = newSkill.GetComponent<DucAnh.Projectiles.Projectile>();
            if (proj != null)
            {
                float damage = stateData.projectileDamage;
                if (boss.IsPhase2()) damage *= boss.phase2DamageMultiplier;
                
                proj.FireProjectile(stateData.projectileSpeed, stateData.projectileTravelDistance, damage);
            }
        }
    }
}
