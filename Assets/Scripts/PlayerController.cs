using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float maxMoveSpeed = 5f; // 최고 이동 속도
    public float acceleration = 30f; // 도달할 때까지의 가속도
    public float deceleration = 30f; // 키를 놓았을 때의 감속도

    [Header("Input")]
    public InputActionReference moveAction; // Input System에서 설정한 액션 연결

    // 다른 상태 클래스에서 접근해야 하므로 public & [HideInInspector]로 선언
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public float moveInputX;

    // 캐릭터가 오른쪽을 보고 있는지
    private bool isFacingRight = true;

    // --- FSM 구조 ---
    private PlayerState currentState;
    public IdleState idleState;
    public WalkState walkState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // 물리 엔진의 기본 마찰력 및 공기 저항 개입을 차단해 스크립트 제어권 확보
        rb.linearDamping = 0f;
        rb.sharedMaterial = new PhysicsMaterial2D { friction = 0f };

        // FSM 상태 인스턴스 생성
        idleState = new IdleState(this);
        walkState = new WalkState(this);
    }

    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
    }

    private void Start()
    {
        // 초기 상태 설정
        ChangeState(idleState);
    }

    private void Update()
    {
        // 입력 시스템(폴링 방식)
        // 이동은 연속적인 입력이므로 Update에서 처리
        if (moveAction != null)
        {
            moveInputX = moveAction.action.ReadValue<Vector2>().x;
        }

        // 이동 방향과 현재 바라보는 방향을 비교해서 뒤집기 판단
        HandleFlip();

        // 현재 상태의 Update 실행
        currentState?.Update();
    }

    private void FixedUpdate()
    {
        // 현재 상태의 물리 연산 실행
        currentState?.FixedUpdate();
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
        if (moveInputX > 0 && !isFacingRight) Flip();
        else if (moveInputX < 0 && isFacingRight) Flip();
    }

    private void Flip()
    {
        // 상태를 반대로 바꾼다
        isFacingRight = !isFacingRight;

        // Transform의 localSacle의 X값을 -1로 곱해서 뒤집는다
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }
}
