using UnityEngine;

public class Boss1 : Entity
{
    public B1_PatrolState patrolState { get; private set; }
    public B1_IdleState idleState { get; private set; }
    public B1_Skill1State skill1State { get; private set; }
    public B1_Skill2State skill2State { get; private set; }
    public B1_MeleeAttackState meleeAttackState { get; private set; }
    public B1_DeadState deadState { get; private set; }
    public B1_Phase2TransitionState phase2TransitionState { get; private set; }

    public bool hasEnteredPhase2 = false;

    [Header("Skill Settings")]
    public float skillCooldown = 5f; // Thời gian chờ chung để tung chiêu tiếp theo
    
    [HideInInspector] public float lastSkillTime;
    [HideInInspector] public int lastSkillUsed = 0; // 0 = chưa dùng, 1 = vừa dùng Skill1, 2 = vừa dùng Skill2

    [Header("Phase 2 Settings (< 50% HP)")]
    public float phase2SpeedMultiplier = 1.5f; // Tốc độ di chuyển tăng x1.5 lần
    public float phase2CooldownMultiplier = 0.5f; // Thời gian chờ tung chiêu giảm còn 1 nửa (đánh nhanh hơn)
    public float phase2DamageMultiplier = 1.5f; // Sát thương nhân x1.5 lần

    public bool IsPhase2()
    {
        if (stats != null)
        {
            return stats.Health.CurrentValue <= stats.Health.MaxValue * 0.5f;
        }
        return false;
    }

    [Header("State Data (Tạm dùng chung Data có sẵn)")]
    public D_IdleState idleStateData;
    public D_MoveState moveStateData;
    public D_MeleeAttack meleeAttackStateData;
    public D_DeadState deadStateData;
    public D_RangedAttackState skill2Data; // Data chứa đạn của Skill 2

    [Header("Attack Positions")]
    public Transform meleeAttackPosition;
    public Transform rangedAttackPosition; // Vị trí đạn bắn ra (tay hoặc ngực)

    [Header("Skill 1 Settings")]
    public D_RangedAttackState skill1Data; // Dùng chung form Data với RangedAttack cho đồng bộ
    public Transform skill1SpawnPosition; // Vị trí sẽ đẻ ra Prefab này

    public override void Awake()
    {
        base.Awake();

        patrolState = new B1_PatrolState(this, stateMachine, "patrol", moveStateData, this);
        idleState = new B1_IdleState(this, stateMachine, "idle", idleStateData, this);
        skill1State = new B1_Skill1State(this, stateMachine, "skill1", skill1SpawnPosition, skill1Data, this);
        skill2State = new B1_Skill2State(this, stateMachine, "skill2", rangedAttackPosition, skill2Data, this);
        meleeAttackState = new B1_MeleeAttackState(this, stateMachine, "meleeAttack", meleeAttackPosition, meleeAttackStateData, this);
        deadState = new B1_DeadState(this, stateMachine, "dead", deadStateData, this);
        phase2TransitionState = new B1_Phase2TransitionState(this, stateMachine, "roar", this); // Chạy animation roar khi hóa điên
    }

    private void Start()
    {
        // Gán thời gian bắt đầu để Boss không xả skill ngay khi vừa spawn
        lastSkillTime = Time.time;
        
        stateMachine.Initialize(patrolState);
    }

    public override void Update()
    {
        base.Update();

        // Kiểm tra chuyển Phase
        if (!hasEnteredPhase2 && IsPhase2() && !isDead)
        {
            hasEnteredPhase2 = true;
            stateMachine.ChangeState(phase2TransitionState);
        }
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (meleeAttackPosition != null && meleeAttackStateData != null)
        {
            Gizmos.DrawWireSphere(meleeAttackPosition.position, meleeAttackStateData.attackRadius);
        }
    }
}
