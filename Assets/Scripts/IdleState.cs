using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController playerController) : base(playerController) { }

    public override void Enter()
    {
        // 고양이가 가만히 있는 애니메이션 재생
        if (player.anim != null) player.anim.SetBool("isWalking", false);
    }

    public override void Update()
    {
        // 점프 입력이 감지되고 땅에 닿아있다면 점프 상태로 전환
        if (player.isGrounded && player.ConsumeJump())
        {
            player.ChangeState(player.jumpState);
            return;
        }
        // 방향키 입력이 감지되면 걷기 상태로 전환
        if (Mathf.Abs(player.moveInputX) > 0.01f)
        {
            player.ChangeState(player.walkState);
        }
    }

    public override void FixedUpdate()
    {
        // 걷다가 멈췄을 때 미끄러지지 않도록 서서히 감속하여 0으로 만듦
        player.rb.linearVelocityX = Mathf.MoveTowards(
            player.rb.linearVelocityX, 0f, player.deceleration * Time.fixedDeltaTime);
    }
}
