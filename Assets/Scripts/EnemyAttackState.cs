using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private Enemy enemy;


    public EnemyAttackState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void EnterState()
    {
        //Debug.Log("Attack State Enter");

        TryAttack();
    }

    public void UpdateState()
    {

        if (enemy.attackSO == null)
        {
            enemy.ChangeState(enemy.chaseState); //공격 스크립터블 오브젝트가 없으면 순찰 상태로 변경
            return;
        }

        if (enemy.playerTarget == null)
        {
            enemy.ChangeState(enemy.patrolState);
            return;
        }

        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.playerTarget.position); //플레이어와 에너미와의 거리 계산

        if (distanceToPlayer > enemy.losePlayerRange)
        {
            enemy.ChangeState(enemy.patrolState);
            return;
        }

        if (distanceToPlayer > enemy.attackSO.enemyAttackRange)
        {
            enemy.ChangeState(enemy.chaseState);
            return;
        }

        bool canAttack = enemy.lastAttackTime >= enemy.attackSO.enemyAttackDelay; //마지막 공격으로부터 공격 딜레이가 끝났는지 확인

        if (canAttack)
        {
            enemy.attackSO.ExcuteAttack(enemy);
            enemy.lastAttackTime = 0f; //공격 직후이므로 공격 시간 초기화
        }

        
    }

    public void FixedUpdateState()
    {
        
    }

    public void ExitState()
    {
        
    }

    private void TryAttack()
    {
        if (enemy.attackSO != null)
        {
            enemy.attackSO.ExcuteAttack(enemy);
            enemy.lastAttackTime = 0f; //공격 직후이므로 공격 시간 초기화
        }
    }
}
