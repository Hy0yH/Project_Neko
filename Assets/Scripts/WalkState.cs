using UnityEngine;

public class WalkState : PlayerState
{
    public WalkState(PlayerController playerController) : base(playerController) { }

    public override void Enter()
    {
        player.playerAudio?.StartWalkLoop();
    }

    public override void Update()
    {
        // 점프 입력이 감지되고 땅에 닿아있다면 점프 상태로 전환
        if (player.isGrounded && player.ConsumeJump())
        {
            player.ChangeState(player.jumpState);
            return;
        }
        // 입력이 없어지면 대기 상태로 전환
        if (Mathf.Abs(player.moveInputX) <= 0.01f)
        {
            player.ChangeState(player.idleState);
        }
    }

    public override void FixedUpdate()
    {
        // linearVelocityX를 목표 속도로 점진적으로 이동
        float targetVelocityX = player.moveInputX * player.maxMoveSpeed;
        player.rb.linearVelocityX = Mathf.MoveTowards(
            player.rb.linearVelocityX, targetVelocityX, player.acceleration * Time.fixedDeltaTime);
    }

    public override void Exit()
    {
        player.playerAudio?.StopWalkLoop();
    }
}
