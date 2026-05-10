using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;

    [Header("AI Data")] //인스펙터에서 SO 데이터들과 플레이어 타켓을 할당
    public EnemyAttackSO attackSO;
    public EnemyMovementSO movementSO;
    public Transform playerTarget;
    public float detectionRange; //플레이어를 감지할 범위

    private IEnemyState currentState; //상태 머신 관리를 위한 내부 변수

    [HideInInspector] public float lastAttackTime; //개인 공격 시간

    private IEnemyState idleState;
    //private IEnemyState chaseState;
    //private IEnemyState patrolState;
    //private IEnemyState attackState;
    
    private Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        idleState = new EnemyIdleState(this);
        //chaseState = new EnemyChaseState(this);
        //patrolState = new EnemyPatrolState(this);
        //attackState = new EnemyAttackState(this);
    }
    
    //상태를 변경해주는 함수
    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null) //현재 상태가 있다면
        {
            currentState.ExitState(); //현재 상태 종료
        }
        currentState = newState; //현재 상태에 새로운 상태 할당
        currentState.EnterState(); //새로운 상태 시작
    }

    private void Update()
    {
        lastAttackTime += Time.deltaTime; //매 프레임마다 공격 시간 누적

        if (currentState != null)
        {
            currentState.UpdateState();
        }
    }
}
