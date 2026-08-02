using UnityEngine;
using Cinemachine;

public class B1_Phase2TransitionState : State
{
    private Boss1 boss;
    private CinemachineVirtualCamera vcam;
    private Transform oldCamTarget;
    private PlayerInputHandler playerInput;
    private DucAnh.CoreSystem.DamageReceiver damageReceiver;

    private float transitionDuration = 2f;
    private SpriteRenderer spriteRenderer;

    private CinemachineBasicMultiChannelPerlin vcamPerlin;
    private float originalAmplitude;
    private float originalFrequency;

    public B1_Phase2TransitionState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Boss1 boss)
        : base(entity, stateMachine, animBoolName)
    {
        this.boss = boss;
    }

    public override void Enter()
    {
        base.Enter();

        // 1. Dừng boss lại
        core.GetCoreComponent<DucAnh.CoreSystem.Movement>()?.SetVelocityZero();

        // 2. Bất tử
        damageReceiver = core.GetCoreComponent<DucAnh.CoreSystem.DamageReceiver>();
        if (damageReceiver != null)
        {
            damageReceiver.isInvincible = true;
        }

        // 3. Đổi màu
        spriteRenderer = boss.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            boss.transform.localScale *= 1.15f; // Phóng to nhẹ cho có uy
        }

        // 4. Cinematic Camera & Khóa Player
        vcam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam != null)
        {
            oldCamTarget = vcam.Follow;
            vcam.Follow = boss.transform; // Lia cam vào Boss

            // Thêm hiệu ứng rung màn hình
            vcamPerlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (vcamPerlin != null)
            {
                originalAmplitude = vcamPerlin.m_AmplitudeGain;
                originalFrequency = vcamPerlin.m_FrequencyGain;
                
                vcamPerlin.m_AmplitudeGain = 5f; // Rung rất mạnh
                vcamPerlin.m_FrequencyGain = 2f; // Tần số rung nhanh
            }
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInput = player.GetComponent<PlayerInputHandler>();
            if (playerInput != null)
            {
                playerInput.enabled = false; // Tạm khóa Input
            }
            
            // Hất văng nhẹ player ra xa bằng hệ thống Knockback chuẩn
            Vector2 knockbackDir = (player.transform.position - boss.transform.position).normalized;
            knockbackDir.y += 0.5f;
            knockbackDir.Normalize();

            var knockBackReceiver = player.GetComponentInChildren<DucAnh.CoreSystem.KnockBackReceiver>();
            if (knockBackReceiver != null)
            {
                knockBackReceiver.KnockBack(new DucAnh.Combat.KnockBack.KnockBackData(knockbackDir, 15f, 1, boss.gameObject));
            }

            // Ép người chơi vào trạng thái Choáng (Stun) để có Animation choáng
            var p = player.GetComponent<Player>();
            if (p != null)
            {
                p.StateMachine.ChangeState(p.PlayerStunState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        if (damageReceiver != null)
        {
            damageReceiver.isInvincible = false;
        }

        if (vcam != null && oldCamTarget != null)
        {
            vcam.Follow = oldCamTarget;

            // Tắt rung màn hình
            if (vcamPerlin != null)
            {
                vcamPerlin.m_AmplitudeGain = originalAmplitude;
                vcamPerlin.m_FrequencyGain = originalFrequency;
            }
        }

        if (playerInput != null)
        {
            playerInput.enabled = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Boss đứng yên gầm
        core.GetCoreComponent<DucAnh.CoreSystem.Movement>()?.SetVelocityZero();

        if (Time.time >= startTime + transitionDuration)
        {
            stateMachine.ChangeState(boss.idleState);
        }
    }
}
