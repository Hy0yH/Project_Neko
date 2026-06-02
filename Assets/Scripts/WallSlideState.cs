using UnityEngine;

public class WallSlideState : PlayerState
{
    public WallSlideState(PlayerController playerController) : base(playerController) { }

    public override void Update()
    {
        // 점프 입력이 감지되면 wallJumpState로 전환
        if (player.ConsumeJump())
        {
            player.ChangeState(player.wallJumpState);
            return;
        }

        // 바닥에 닿아 있고 위로 상승하는 중이 아니면 대기 상태로 전환
        if (player.isGrounded && player.rb.linearVelocityY <= 0f)
        {
            player.isWallJumping = false; // 벽 점프 상태 해제

            // 점프 애니메이션 종료
            if (player.anim != null) player.anim.SetBool("isJumping", false);

            // 효과음
            player.playerAudio.PlayJumpLand();

            player.ChangeState(player.idleState);
            return;
        }

        // 벽에서 멀어지면 점프 상태로 복귀
        if (!player.isTouchingWall)
        {
            player.ChangeState(player.jumpState);
        }
    }

    public override void FixedUpdate()
    {
        // 하강 속도가 wallSlideSpeed를 넘지 못하도록 고정
        player.rb.linearVelocityY = Mathf.Clamp(player.rb.linearVelocityY, -player.wallSlideSpeed, float.MaxValue);
    }
}
