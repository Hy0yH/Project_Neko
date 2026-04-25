using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float maxMoveSpeed = 10f; // 최고 이동 속도
    public float acceleration = 60f; // 도달할 때까지의 가속도
    public float deceleration = 60f; // 키를 놓았을 때의 감속도

    [Header("Input")]
    // Input System에서 설정한 액션 연결
    public InputActionReference moveAction;

    private Rigidbody2D rb;
    private float moveInputX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // 물리 엔진의 기본 마찰력 및 공기 저항 개입을 차단해 스크립트 제어권 확보
        rb.linearDamping = 0f;
        rb.sharedMaterial = new PhysicsMaterial2D { friction = 0f };
    }

    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
    }

    private void Update()
    {
        // 입력 시스템(폴링 방식)
        // 이동은 연속적인 입력이므로 Update에서 처리
        if(moveAction != null)
        {
            moveInputX = moveAction.action.ReadValue<Vector2>().x;
        }
    }

    private void FixedUpdate()
    {
        // 물리 연산 적용
        HandleHorizontalMovement();
    }

    private void HandleHorizontalMovement()
    {
        // 목표로 하는 수평 속도
        float targetVelocityX = moveInputX * maxMoveSpeed;

        // 현재 X축 속도
        float currentVelocityX = rb.linearVelocityX;

        // 플레이어의 입력이 있는지(가속) 없는지(정지)에 따라 적용할 변화율 결정
        float accelRate = (Mathf.Abs(targetVelocityX) > 0.01f) ? acceleration : deceleration;
        
        // 현재 속도에서 목표 속도를 향해 지정된 가속도/감속도만큼 부드럽게 값을 변경
        float newVelocityX = (Mathf.MoveTowards(currentVelocityX, targetVelocityX, accelRate * Time.fixedDeltaTime));

        // Unity 6 방식: 벡터를 새로 할당하지 않고 X축의 linearVelocityX만 직접 덮어씌움
        rb.linearVelocityX = newVelocityX;
    }
}
