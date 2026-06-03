using UnityEngine;

public class WallJumpState : PlayerState
{
    private float lockTimer = 0f; // 입력 잠금 타이머
    public WallJumpState(PlayerController playerController) : base(playerController) { }

    public override void Enter()
    {
        // 효과음
        player.playerAudio?.PlayJumpTakeoff();

        player.isWallJumping = true;

        // 타이머 시작
        lockTimer = player.wallJumpInputLockTime;

        // 반대 방향으로 점프
        player.rb.linearVelocity = new Vector2(-player.facingDirection * player.wallJumpForce.x, player.wallJumpForce.y);

        // 캐릭터 방향 반대로 설정
        player.Flip();
    }

    public override void Update()
    {
        // 타이머 감소
        lockTimer -= Time.deltaTime;

        // 타이머가 끝나면 입력 잠금 해제
        if (lockTimer <= 0f)
        {
            player.isWallJumping = false;
            player.ChangeState(player.jumpState);
        }
    }

    public override void Exit()
    {
        player.isWallJumping = false;
    }
}
