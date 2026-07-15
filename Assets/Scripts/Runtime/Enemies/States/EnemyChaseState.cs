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

        float distanceX = enemy.playerTarget.position.x - enemy.transform.position.x;
        float absDistanceX = Mathf.Abs(distanceX);
        float distanceY = Mathf.Abs(enemy.playerTarget.position.y - enemy.transform.position.y);

        if (absDistanceX > enemy.losePlayerRange || distanceY > enemy.detectionHeight)
        {
            enemy.ChangeState(enemy.patrolState);
            return;
        }

        if (enemy.attackSO != null)
        {
            bool isWithinAttackRange = absDistanceX <= enemy.attackSO.enemyAttackRange; //플레이어가 공격 범위 안에 있는지 확인
            bool canAttack = enemy.lastAttackTime >= enemy.attackSO.enemyAttackDelay; //마지막 공격으로부터 공격 딜레이가 끝났는지 확인

            if (isWithinAttackRange && canAttack)
            {
                enemy.ChangeState(enemy.attackState);
                return;
            } 
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
