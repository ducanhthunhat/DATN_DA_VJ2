using UnityEngine;

public class B1_IdleState : IdleState
{
    private Boss1 boss;

    public B1_IdleState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, Boss1 boss) 
        : base(entity, stateMachine, animBoolName, stateData)
    {
        this.boss = boss;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Đứng thở (Idle) xong thì quay lại đi tuần tra
        if (isIdleTimeOver)
        {
            stateMachine.ChangeState(boss.patrolState);
        }
    }
}
