using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    private Enemy enemy;

    public EnemyChaseState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void EnterState()
    {
        
    }

    public void UpdateState()
    {
        
        if (enemy.playerTarget == null)
        {
            enemy.ChangeState(enemy.patrolState); //플레이어를 잃으면 순찰 상태로 변경
            return;
        }

        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.playerTarget.position);

        //Debug.Log($"Distance To Player: {distanceToPlayer}, Lose Range: {enemy.losePlayerRange}"); 문제가 생겼을 때, 한번씩 로그 출력을 통해 문제 원인 파악


        if (distanceToPlayer > enemy.losePlayerRange)
        {
            enemy.ChangeState(enemy.patrolState); //플레이어를 잃으면 순찰 상태로 변경
            return;
        }

        if (isWithinAttckRange && canAttack)
        {
            enemy.ChangeState(enemy.attackState);
        }
    }

    public void FixedUpdateState()
    {
        if (enemy.playerTarget == null) return;
        if (enemy.movementSO == null) return;
        if (enemy.rigid == null) return;

        float dirX = Mathf.Sign(enemy.playerTarget.position.x - enemy.transform.position.x);

        SetFacingDirection(dirX);

        enemy.movementSO.ExcuteMove(enemy);

        //나중에 공격 로직 넣을 것.

    }

    public void ExitState()
    {
        
    }

    private void SetFacingDirection(float dirX)
    {
        if (dirX == 0) return;

        Vector3 scale = enemy.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dirX;
        enemy.transform.localScale = scale;
    }
}
