using UnityEngine;

public class JumpState : PlayerState
{
    public JumpState(PlayerController playerController) : base(playerController) { }

    private float jumpCutMultiplier = 0.5f; // 얼마나 속도를 깎을지

    public override void Enter()
    {
        // 점프 시작
        // 점프 애니메이션 시작하고 최대 점프 속도로 설정
        if (player.anim != null) player.anim.SetBool("isJumping", true);

        // 땅에 있을 때만(벽점프시 성립 x)
        if (player.isGrounded)
            player.rb.linearVelocityY = player.maxJumpForce;
    }
    public override void Update()
    {
        // 벽에 닿아 있으면 wallSlideState로 전환
        if (player.isTouchingWall)
        {
            player.ChangeState(player.wallSlideState);
            return;
        }

        // 땅에 닿으면 대기/걷기 상태로 전환
        if (player.isGrounded && player.rb.linearVelocityY <= 0f)
        {
            player.isWallJumping = false; // 벽 점프 상태 해제

            // 점프 애니메이션 종료
            if (player.anim != null) player.anim.SetBool("isJumping", false);

            if (Mathf.Abs(player.moveInputX) > 0.01f) player.ChangeState(player.walkState);
            else player.ChangeState(player.idleState);
        }
    }
    public override void FixedUpdate()
    {
        // 가변 점프 구현
        // 캐릭터가 상승중인데 (linearVelocityY > 0) 점프 버튼이 떼어지면(!isPressed)
        if (player.rb.linearVelocityY > 0 && !player.jumpHeld)
        {
            player.rb.linearVelocityY *= jumpCutMultiplier; // 상승 속도를 깎아 낮은 점프를 만듦
        }

        // 공중에서 좌우 이동 허용
        if (!player.isWallJumping)
        {
            float targetVelocityX = player.moveInputX * player.maxMoveSpeed;
            player.rb.linearVelocityX = Mathf.MoveTowards(
                player.rb.linearVelocityX, targetVelocityX, player.acceleration * Time.fixedDeltaTime);
        }
    }
}
