using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShineState : EnemyState
{
    public EnemyShineState(EnemyStateMachine _stateMachine, Enemy _enemy, string _animBoolName) : base(_stateMachine, _enemy, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Exit()
    {
        base.Exit();
    }
}
