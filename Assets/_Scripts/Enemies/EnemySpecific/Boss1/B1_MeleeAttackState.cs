using UnityEngine;

public class B1_MeleeAttackState : MeleeAttackState
{
    private Boss1 boss;

    public B1_MeleeAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttack stateData, Boss1 boss) 
        : base(entity, stateMachine, animBoolName, attackPosition, stateData)
    {
        this.boss = boss;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            // Bất kể người chơi còn ở gần hay không, đánh xong 1 cú là phải đứng thở
            stateMachine.ChangeState(boss.idleState);
        }
    }
}
