using UnityEngine;
using System.Collections;
using System;

public class Enemy : MonoBehaviour, IDamageable
{
    public int health;

    public static event Action<Enemy> OnEnemyDied; //적이 죽었을 때 호출되는 이벤트

    private bool isEnemyDead;

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
    [HideInInspector] public float attackDirectionX; //공격 방향 X

    [HideInInspector] public IEnemyState idleState;
    [HideInInspector] public IEnemyState chaseState;
    [HideInInspector] public IEnemyState patrolState;
    [HideInInspector] public IEnemyState attackState;
    
    [HideInInspector]public Rigidbody2D rigid;
    [HideInInspector] public Animator anim;

    [SerializeField] private float hitFalshDuration = 0.1f; //피격 시 깜빡이는 시간
    [SerializeField] private Material hitFlashMaterial; //피격 시 사용할 머티리얼

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip hitClip;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial; //원래 머티리얼을 저장할 변수
    private Coroutine hitFlashCoroutine;

    private bool isPaused;
    private float previousAnimatorSpeed;

    private void Start()
    {
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                playerTarget = player.transform;
            }
        }
        ChangeState(patrolState);
    }

    public void SetPaused(bool paused)
    {
        if (isPaused == paused)
            return;

        isPaused = paused;

        if (rigid != null)
        {
            rigid.linearVelocity = Vector2.zero;
            rigid.angularVelocity = 0f;
        }

        if (anim != null)
        {
            if (paused)
            {
                previousAnimatorSpeed = anim.speed;
                anim.speed = 0f;
            }
            else
            {
                anim.speed = previousAnimatorSpeed;
            }
        }
    }


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material; //원래 머티리얼 저장
        }

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
        if (isPaused)
            return;

        lastAttackTime += Time.deltaTime; //매 프레임마다 공격 시간 누적

        if (currentState != null)
        {
            currentState.UpdateState();
        }
    }

    private void FixedUpdate()
    {
        if (isPaused)
            return;

        if (currentState != null)
        {
            //물리 엔진을 이용한 이동 로직은 여기에서 처리한다.
            currentState.FixedUpdateState();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isEnemyDead) return;

        health -= damage;

        if (sfxSource != null && hitClip != null)
        {
            sfxSource.PlayOneShot(hitClip);
        }

        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
        }

        hitFlashCoroutine = StartCoroutine(HitFlash());

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isEnemyDead) return;

        isEnemyDead = true;

        //Debug.Log($"Enemy died event sent: {name}");

        OnEnemyDied?.Invoke(this); //적이 죽었을 때 이벤트 호출

        Destroy(gameObject); //적 오브젝트 제거
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void TryDamagePlayer(Collider2D other)
    {
        if (isPaused || attackSO == null)
            return;

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
            return;

        playerHealth.TakeDamage(attackSO.enemyDamage);
    }

    private IEnumerator HitFlash()
    {
        if (spriteRenderer == null) yield break;
        if (hitFlashMaterial == null) yield break;

        spriteRenderer.material = hitFlashMaterial; //피격 시 지정된 머티리얼로 변경

        yield return new WaitForSeconds(hitFalshDuration); //잠시 대기

        spriteRenderer.material = originalMaterial; //원래 머티리얼로 복원
        hitFlashCoroutine = null; //코루틴 종료 표시
    }

    


}
