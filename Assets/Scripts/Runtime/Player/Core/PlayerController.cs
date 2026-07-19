using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float maxMoveSpeed = 5f; // 최고 이동 속도
    public float acceleration = 30f; // 도달할 때까지의 가속도
    public float deceleration = 30f; // 키를 놓았을 때의 감속도
    public float maxJumpForce = 3f; // 최대 점프 힘
    public Vector2 wallJumpForce = new Vector2(2f, 3f); // 벽 점프 시 가해지는 힘 (X, Y)
    public float wallJumpInputLockTime = 0.3f; // 벽 점프 후 입력 잠금 시간
    public float maxFallSpeed = 20f;

    [Header("Input")]
    public InputActionReference moveAction; // Input System에서 설정한 액션 연결
    public InputActionReference jumpAction;

    [Header("Environment")]
    public Transform groundCheck; // 발 밑 오브젝트
    [SerializeField] private float groundCheckRadius = 0.2f; // 땅 체크 반경
    public LayerMask groundLayer; // 땅 레이어
    public Transform wallCheck; // 벽 체크용 오브젝트
    [SerializeField] private float wallCheckRadius = 0.2f; // 벽 체크 반경
    public LayerMask wallLayer; // 벽 레이어

    public float wallSlideSpeed = 3f; // 벽 타는 속도

    // 다른 상태 클래스에서 접근해야 하므로 public & [HideInInspector]로 선언
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public float moveInputX;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool jumpInputTriggered;
    [HideInInspector] public bool jumpHeld;
    [HideInInspector] public bool isTouchingWall;
    [HideInInspector] public bool isWallJumping;
    [HideInInspector] public int facingDirection = 1; // 1은 오른쪽, -1은 왼쪽을 보고 있는 상태
    [HideInInspector] public PlayerAudio playerAudio;
    [HideInInspector] public PlayerForm playerForm;

    // --- FSM 구조 ---
    private PlayerState currentState;
    public IdleState idleState;
    public WalkState walkState;
    public JumpState jumpState;
    public WallSlideState wallSlideState;
    public WallJumpState wallJumpState;
    public DeadState deadState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // 물리 엔진의 기본 마찰력 및 공기 저항 개입을 차단해 스크립트 제어권 확보
        rb.sharedMaterial = new PhysicsMaterial2D { friction = 0f };

        // FSM 상태 인스턴스 생성
        idleState = new IdleState(this);
        walkState = new WalkState(this);
        jumpState = new JumpState(this);
        wallSlideState = new WallSlideState(this);
        wallJumpState = new WallJumpState(this);
        deadState = new DeadState(this);

        isWallJumping = false;

        playerAudio = GetComponent<PlayerAudio>();
        playerForm = GetComponent<PlayerForm>();
    }

    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
        if (jumpAction != null) jumpAction.action.Enable();
        PlayerHealth.OnDeath += HandleDeath;
        PlayerHealth.OnRespawn += HandleRespawn;
    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
        if (jumpAction != null) jumpAction.action.Disable();
        PlayerHealth.OnDeath -= HandleDeath;
        PlayerHealth.OnRespawn -= HandleRespawn;
    }

    private void Start()
    {
        // 초기 상태 설정
        ChangeState(idleState);
    }

    private void Update()
    {
        // 바닥 판정
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) != null;

        // 벽 닿아있는지 판정
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer) != null;

        if (currentState != deadState)
        {
            // 점프 입력 감지
            if (jumpAction != null)
            {
                jumpInputTriggered = jumpAction.action.WasPressedThisFrame();
                jumpHeld = jumpAction.action.IsPressed();
            }

            // 낙하 속도 측정
            if (anim != null)
                anim.SetFloat("velocityY", rb.linearVelocityY);

            // 이동 입력 감지
            if (moveAction != null)
            {
                moveInputX = moveAction.action.ReadValue<Vector2>().x;
                if (anim != null && moveInputX != 0f)
                    anim.SetBool("isWalking", true);
                else if (moveInputX == 0f)
                    anim.SetBool("isWalking", false);
            }

            // 이동 방향과 현재 바라보는 방향을 비교해서 뒤집기 판단
            if (!isWallJumping)
                HandleFlip();
        }

        // 현재 상태의 Update 실행
        currentState?.Update();
    }

    private void FixedUpdate()
    {
        // 현재 상태의 물리 연산 실행
        currentState?.FixedUpdate();

        // 최대 낙하 속도 설정
        if (rb.linearVelocityY < -maxFallSpeed)
            rb.linearVelocityY = -maxFallSpeed;
    }

    public void ChangeState(PlayerState newState)
    {
        if (currentState != null)
        {
            currentState.Exit(); // 기존 상태 종료
        }

        currentState = newState;
        currentState.Enter(); // 새 상태 시작
    }

    private void HandleFlip()
    {
        if (moveInputX > 0 && facingDirection == -1) Flip();
        else if (moveInputX < 0 && facingDirection == 1) Flip();
    }

    public void Flip()
    {
        // 보고있는 방향 반전
        facingDirection *= -1;

        // Transform의 localSacle의 X값을 -1로 곱해서 뒤집는다
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    public bool ConsumeJump()
    {
        if (!jumpInputTriggered) return false;
        jumpInputTriggered = false; // 점프 입력 소비
        return true;
    }

    private void HandleDeath()
    {
        ChangeState(deadState);
    }

    private void HandleRespawn()
    {
        anim.SetBool("isDead", false);
        rb.linearVelocity = Vector2.zero;
        ChangeState(idleState);

        GetComponent<PlayerCombat>().enabled = true;
        GetComponent<PlayerInventory>().enabled = true;
    }

    public void SetControlLocked(bool locked)
    {
        if (locked)
        {
            moveInputX = 0f;
            jumpInputTriggered = false;
            jumpHeld = false;

            if (rb != null)
                rb.linearVelocityX = 0f;

            // 걷기 상태와 발소리도 멈추도록 설정
            if (idleState != null)
                ChangeState(idleState);
        }

        enabled = !locked;
    }
}
