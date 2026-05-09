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



        //일정시간이 지나면 순찰 상태로 변경
        timer += Time.deltaTime;
        if (timer >= idleTime)
        {
            //enemy.ChangeState(enemy.patrolState);
        }

    }

    public void ExitState()
    {

    }
}
