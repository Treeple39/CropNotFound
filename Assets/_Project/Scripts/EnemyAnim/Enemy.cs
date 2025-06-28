using Assets._Project.Scripts.EnemyAnim;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]public Rigidbody2D rb {  get; private set; }
    public Animator anim { get; private set; }

    public BaseMovement movement { get; private set; }

    public EnemyStateMachine stateMachine { get; private set; }

    #region AnimationSet
    public EnemyIdleState idleState { get; private set; }
    public EnemyDashState dashState { get; private set; }
    public EnemyFleeState fleeState { get; private set; }

    public EnemyDieState dieState { get; private set; }
    public EnemyShineState shineState { get; private set; }
    public EnemyDizzState dizzState { get; private set; }
    #endregion

    public bool dead;

    public virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        anim = GetComponent<Animator>();
        movement = GetComponent<BaseMovement>();

        idleState = new EnemyIdleState(stateMachine, this, "Idle");
        dashState = new EnemyDashState(stateMachine, this, "Dash");
        fleeState = new EnemyFleeState(stateMachine, this, "Flee");
        dieState = new EnemyDieState(stateMachine, this, "Die");
        shineState = new EnemyShineState(stateMachine, this, "Shine");
        dizzState = new EnemyDizzState(stateMachine, this, "Dizz");
        stateMachine.Initialize(idleState);
    }

    public virtual void Die()
    {
        stateMachine.ChangeState(dieState);
    }

}
