using UnityEngine;

public class WallSlideState : PlayerState
{
    public WallSlideState(PlayerController playerController) : base(playerController) { }

    public override void Update()
    {
        // 바닥에 닿으면 대기 상태로 전환
        if (player.isGrounded)
        {
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
