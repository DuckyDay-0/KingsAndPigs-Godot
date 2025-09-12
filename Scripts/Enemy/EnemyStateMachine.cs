using Godot;
using System;

public partial class EnemyStateMachine 
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Cooldown
    }

    public EnemyState CurrentState { get; private set; } = EnemyState.Idle;

    private float stateTimer;

    public void ChangeState(EnemyState newState, float duration = 0f)
    { 
        CurrentState = newState;
        stateTimer = duration;
    }

    public void Update(float delta)
    {
        stateTimer -= delta;
    }

    public bool IsStateFinished()
    {
        return stateTimer < 0f;
    }
}
