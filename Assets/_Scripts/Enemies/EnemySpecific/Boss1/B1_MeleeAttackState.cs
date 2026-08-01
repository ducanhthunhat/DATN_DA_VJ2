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

    public override void TriggerAttack()
    {
        float originalDamage = stateData.attackDamage;
        if (boss.IsPhase2())
        {
            stateData.attackDamage *= boss.phase2DamageMultiplier;
        }
        
        base.TriggerAttack();
        
        stateData.attackDamage = originalDamage; // Trả lại như cũ để không bị cộng dồn
    }
}
