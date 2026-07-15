using Unity.VisualScripting;
using UnityEngine;

public class EnemyPatrolState : IEnemyState
{
    private Enemy enemy;

    private int currentPatrolPointIndex = 0; //현재 목표 위치
    private int patrolDirection = 1; //방향 설정 오른쪽 = 1, 왼쪽 = -1

    public EnemyPatrolState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void EnterState()
    {
        currentPatrolPointIndex = 0;
        patrolDirection = 1;
    }

    public void UpdateState()
    {
        if (CanDetectPlayer())
        {
            enemy.ChangeState(enemy.chaseState); //플레이어를 감지하면 추적 상태로 변경
            Debug.Log("플레이어를 발견했습니다!");
            return;
        }

        if (enemy.patrolPoints == null || enemy.patrolPoints.Length == 0)
        {
            return;
        }

        Transform targetPoint = enemy.patrolPoints[currentPatrolPointIndex];

        float dirX = Mathf.Abs(enemy.transform.position.x - targetPoint.position.x); //현재 적의 x좌표와 목표 지점의 x좌표 차이

        if(dirX <= enemy.patrolReachDistance) // 목표 지점에 가까워지면 다음 포인트로 변경
        {
            MoveToNextPatrolPoint(); // 다음 포인트로 이동
        }
    }

    public void FixedUpdateState()
    {
        if (!HasVaildPatrolPoints()) return;
        if (enemy.movementSO == null) return;
        if (enemy.rigid == null) return;

        Transform targetPoint = enemy.patrolPoints[currentPatrolPointIndex];

        float dirX = Mathf.Sign(targetPoint.transform.position.x - enemy.transform.position.x); //목표 포인트가 현재 위치 기준 왼쪽인지 오른쪽인지 판단

        SetFacingDirection(dirX);

        enemy.movementSO.ExcuteMove(enemy);
    }

    public void ExitState()
    {
        // 다른 상태로 변하는 로직 추가
    }

    private bool HasVaildPatrolPoints()
    {
        return enemy.patrolPoints != null && enemy.patrolPoints.Length >= 2; // 왕복 순찰이 최소 2개
    }

    private void MoveToNextPatrolPoint()
    {
        currentPatrolPointIndex += patrolDirection; //현재 목표 지점에서 다음 포인트로 이동

        if (currentPatrolPointIndex >= enemy.patrolPoints.Length) //마지막 포인트를 넘는다면 반대 방향으로 전환
        {
            patrolDirection = -1;
            currentPatrolPointIndex = enemy.patrolPoints.Length - 2;
        }
        else if (currentPatrolPointIndex < 0) //첫 번째 포인트를 벗어난다면 다시 반대 방향으로 전환
        {
            patrolDirection = 1;
            currentPatrolPointIndex = 1;
        }
    }

    private void SetFacingDirection(float dirX)
    {
        if (dirX == 0) //방향 값이 0이면 굳이 바꾸지 않음
        {
            return;
        }

        Vector3 scale = enemy.transform.localScale;

        scale.x = Mathf.Abs(scale.x) * dirX; // enemy가 오른쪽을 보면 양수, 왼쪽이면 음수

        enemy.transform.localScale = scale;
    }

    private bool CanDetectPlayer()
    {
        if (enemy.playerTarget == null)
        {
            return false;
        }

        Vector2 enemyPosition = enemy.transform.position;
        Vector2 playerPosition = enemy.playerTarget.position;

        float facingDirection = Mathf.Sign(enemy.transform.localScale.x);

        float distanceX = playerPosition.x - enemyPosition.x;
        float distanceY = Mathf.Abs(playerPosition.y - enemyPosition.y);

        // 플레이어가 적이 바라보는 방향에 있는지 확인
        bool isPlayerInFront = Mathf.Sign(distanceX) == facingDirection;

        // X축 감지 거리 안에 있는지 확인
        bool isWithinXRange = Mathf.Abs(distanceX) <= enemy.detectionRange;

        // Y축 차이가 작아 같은 층으로 볼 수 있는지 확인
        bool isWithinYRange = distanceY <= enemy.detectionHeight;

        return isPlayerInFront && isWithinXRange && isWithinYRange;
    }
}
