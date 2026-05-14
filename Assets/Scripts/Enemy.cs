using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;

    [Header("AI Data")] //인스펙터에서 SO 데이터들과 플레이어 타켓을 할당
    public EnemyAttackSO attackSO;
    public EnemyMovementSO movementSO;
    public Transform playerTarget;

    [Header("Detection Settings")] //플레이어를 감지할 설정
    public float detectionRange; //플레이어를 감지할 범위
    public float detectionHeight; //플레이어를 감지할 높이
    public float losePlayerRange; //플레이어를 잃을 범위

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolReachDistance = 0.15f; //목표 지점에 도착했다고 판단할 거리

    [HideInInspector] public IEnemyState currentState; //상태 머신 관리를 위한 내부 변수

    [HideInInspector] public float lastAttackTime; //개인 공격 시간

    [HideInInspector] public IEnemyState idleState;
    [HideInInspector] public IEnemyState chaseState;
    [HideInInspector] public IEnemyState patrolState;
    [HideInInspector] public IEnemyState attackState;
    
    [HideInInspector]public Rigidbody2D rigid;

    private void Start()
    {
        ChangeState(patrolState);
    }


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        idleState = new EnemyIdleState(this);
        chaseState = new EnemyChaseState(this);
        patrolState = new EnemyPatrolState(this);
        attackState = new EnemyAttackState(this);
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

    private void FixedUpdate()
    {
        if (currentState != null)
        {
            //물리 엔진을 이용한 이동 로직은 여기에서 처리한다.
            currentState.FixedUpdateState();
        }
    }
}
