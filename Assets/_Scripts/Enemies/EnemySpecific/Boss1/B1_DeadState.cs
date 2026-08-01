using UnityEngine;

public class B1_DeadState : State
{
    private Boss1 boss;
    private D_DeadState stateData;

    public B1_DeadState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData, Boss1 boss) 
        : base(entity, stateMachine, animBoolName)
    {
        this.boss = boss;
        this.stateData = stateData;
    }

    public override void Enter()
    {
        base.Enter(); 
        
        var movement = core.GetCoreComponent<DucAnh.CoreSystem.Movement>();
        if (movement != null)
        {
            // Ép buộc mở khóa vận tốc (để hủy bỏ hiệu ứng Knockback nếu có)
            movement.CanSetVelocity = true;
            movement.SetVelocityZero();

            // Khóa cứng vật lý, tắt luôn trọng lực để cái xác không bị rơi xuyên map
            if (movement.RB != null)
            {
                movement.RB.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        // Tắt TOÀN BỘ Collider trên người Boss (Hitbox, Hurtbox, v.v.)
        // Để người chơi có thể đi xuyên qua cái xác mà không bị vướng hay mất máu
        Collider2D[] allColliders = core.transform.parent.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in allColliders)
        {
            col.enabled = false;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        // Khóa chết vận tốc mỗi khung hình để không lực nào đẩy được cái xác đi
        core.GetCoreComponent<DucAnh.CoreSystem.Movement>()?.SetVelocityZero();
    }
}
