using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    private Enemy enemy;
    private float timer; //스톱워치(얼마나 대기하는지를 측정)
    private float idleTime; //대기 시간

    public EnemyIdleState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void EnterState()
    {
        timer = 0f;
        //추후에 애니메이션 추가할 때 넣을 곳
    }

    public void UpdateState()
    {
        //여기에 플레이어 감지 로직 넣어야 함.
        if (enemy.playerTarget != null)
        {
            float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.playerTarget.position); //플레이어와 에너미와의 거리 계산

            if (distanceToPlayer <= enemy.detectionRange)
            {
                // 추적 로직 추가
                //enemy.ChangeState(enemy.chaseState);

                return; // 로직이 변경되었기 때문에 아래의 코드는 무시
            }

        }

        //일정시간이 지나면 순찰 상태로 변경
        timer += Time.deltaTime;
        if (timer >= idleTime)
        {
            enemy.ChangeState(enemy.patrolState);
        }

    }

    public void FixedUpdateState()
    {
        
    }

    public void ExitState()
    {

    }
}
