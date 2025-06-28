using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Assets._Project.Scripts.EnemyAnim
{
    public class EnemyDieState : EnemyState
    {
        public EnemyDieState(EnemyStateMachine _stateMachine, Enemy _enemy, string _animBoolName) : base(_stateMachine, _enemy, _animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            //DieAnim dieAnim = new DieAnim();
            //dieAnim.StarExplode(enemy.transform.position);
            Delete();

        }
        private IEnumerator Delete()
        {
            yield return new WaitForSeconds(0.5f);
            Object.Destroy(this.enemy.gameObject);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}