using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State
{
    protected D_DeadState stateData;

    public DeadState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData) : base(etity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        Vector3 spawnPosition = entity.transform.position;
        if (entity.aliveGO != null)
        {
            spawnPosition = entity.aliveGO.transform.position;
        }

        if (stateData.deathBloodParticle != null)
        {
            GameObject.Instantiate(stateData.deathBloodParticle, spawnPosition, stateData.deathBloodParticle.transform.rotation);
        }

        if (stateData.deathChunkParticle != null)
        {
            GameObject.Instantiate(stateData.deathChunkParticle, spawnPosition, stateData.deathChunkParticle.transform.rotation);
        }

        entity.gameObject.SetActive(false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}