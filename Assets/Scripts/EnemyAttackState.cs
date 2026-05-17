using UnityEngine;

public class EnemyAttackState : IEnemyState
{

    private enum AttackPhase
    {
        Active,
        Recovery
    }
    private Enemy enemy;

    private AttackPhase currentPhase;
    private float phaseTimer;

    public EnemyAttackState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void EnterState()
    {
        if (enemy.attackSO == null)
        {
            enemy.ChangeState(enemy.chaseState);
            return;
        }

        //Debug.Log("Attack State Enter");
        currentPhase = AttackPhase.Active;
        phaseTimer = 0f;

        enemy.attackSO.ExcuteAttack(enemy);
        enemy.lastAttackTime = 0f;

        enemy.attackDirectionX = Mathf.Sign(enemy.playerTarget.position.x - enemy.transform.position.x);

        if (enemy.attackDirectionX == 0)
        {
            enemy.attackDirectionX = Mathf.Sign(enemy.transform.localScale.x);
        }
    }

    public void UpdateState()
    {

        if (enemy.attackSO == null)
        {
            enemy.ChangeState(enemy.chaseState); //공격 스크립터블 오브젝트가 없으면 순찰 상태로 변경
            return;
        }

        phaseTimer += Time.deltaTime;

        if (currentPhase == AttackPhase.Active)
        {
            if (phaseTimer >= enemy.attackSO.enemyAttackDuration)
            {
                StopCurrentMovement();
                currentPhase = AttackPhase.Recovery;
                phaseTimer = 0f;
            }

            return;
        }

        if (currentPhase == AttackPhase.Recovery)
        {
            StopCurrentMovement();

            if (phaseTimer >= enemy.attackSO.enemyRecoveryTime)
            {
                DecideNextState();
            }

            return;
        }
    }

    public void FixedUpdateState()
    {
        if (enemy.attackSO == null) return;

        if (currentPhase == AttackPhase.Active)
        {
            enemy.attackSO.ExcuteAttack(enemy);
        }
        else if (currentPhase == AttackPhase.Recovery)
        {
            StopCurrentMovement();
        }
    }

    public void ExitState()
    {
        
    }

    private void StopCurrentMovement()
    {
        if (enemy.rigid == null) return;

        enemy.rigid.linearVelocity = new Vector2(0f, enemy.rigid.linearVelocity.y);
    }

    private void DecideNextState()
    {
        float distanceX = enemy.playerTarget.position.x - enemy.transform.position.x;
        float absDistanceX = Mathf.Abs(distanceX);
        float distanceY = Mathf.Abs(enemy.playerTarget.position.y - enemy.transform.position.y);

        if (absDistanceX > enemy.losePlayerRange || distanceY > enemy.detectionHeight)
        {
            enemy.ChangeState(enemy.patrolState);
            return;
        }

        enemy.ChangeState(enemy.chaseState);

    }
}
