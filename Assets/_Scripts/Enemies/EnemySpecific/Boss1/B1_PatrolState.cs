using UnityEngine;

public class B1_PatrolState : MoveState
{
    private Boss1 boss;

    public B1_PatrolState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, Boss1 boss) 
        : base(entity, stateMachine, animBoolName, stateData)
    {
        this.boss = boss;
    }

    private float lastFlipTime;

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 1. Kiểm tra tử vong
        if (core.GetCoreComponent<DucAnh.CoreSystem.Stats>().Health.CurrentValue <= 0)
        {
            stateMachine.ChangeState(boss.deadState);
            return;
        }

        // 2. Kiểm tra nếu đã hết thời gian chờ (Có tính Phase 2)
        float currentCooldown = boss.skillCooldown;
        if (boss.IsPhase2())
        {
            currentCooldown *= boss.phase2CooldownMultiplier;
        }

        if (Time.time >= boss.lastSkillTime + currentCooldown)
        {
            // Luân phiên: Nếu trước đó chưa dùng hoặc vừa dùng Skill 2, thì giờ dùng Skill 1
            if (boss.lastSkillUsed != 1)
            {
                stateMachine.ChangeState(boss.skill1State);
            }
            // Ngược lại, nếu trước đó vừa dùng Skill 1, thì giờ dùng Skill 2
            else
            {
                stateMachine.ChangeState(boss.skill2State);
            }
            return;
        }

        // 4. Nếu có player ở gần -> Cận chiến
        if (boss.CheckPlayerInCloseRangeAction())
        {
            stateMachine.ChangeState(boss.meleeAttackState);
            return;
        }

        // 5. Quay đầu khi đụng tường (Kèm thời gian chờ 0.5 giây để tránh bị giật lật liên tục)
        if ((isDetectingWall || !isDetectingLedge) && Time.time >= lastFlipTime + 0.5f)
        {
            core.GetCoreComponent<DucAnh.CoreSystem.Movement>()?.Flip();
            lastFlipTime = Time.time;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate(); // Hàm base đã set tốc độ mặc định

        // Ở đây chúng ta ghi đè lại tốc độ nếu đang ở Phase 2
        float currentSpeed = stateData.movementSpeed;
        if (boss.IsPhase2())
        {
            currentSpeed *= boss.phase2SpeedMultiplier;
        }

        var movement = core.GetCoreComponent<DucAnh.CoreSystem.Movement>();
        if (movement != null)
        {
            movement.SetVelocityX(currentSpeed * movement.FacingDirection);
        }
    }
}
