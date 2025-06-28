using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Rigidbody2D rb {  get; private set; }
    public Animator anim { get; private set; }

    public BaseMovement movement { get; private set; }

    public EnemyStateMachine stateMachine { get; private set; }

    #region AnimationSet
    public EnemyIdleState idleState { get; private set; }
    public EnemyDashState dashState { get; private set; }
    public EnemyFleeState fleeState { get; private set; }
    #endregion

    public virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        anim = GetComponent<Animator>();
        movement = GetComponent<BaseMovement>();

        idleState = new EnemyIdleState(stateMachine, this, "Idle");
        dashState = new EnemyDashState(stateMachine, this, "dash");
        fleeState = new EnemyFleeState(stateMachine, this, "Flee");
        stateMachine.Initialize(idleState);
    }


}
