using UnityEngine;

public class DeadState : PlayerState
{
    public DeadState(PlayerController playerController) : base(playerController) { }

    public override void Enter()
    {
        // player.rb.linearVelocityX = 0f;

        if (player.anim != null) player.anim.SetBool("isDead", true);

        player.playerAudio?.PlayStun();
    }

    public override void Update()
    {
        
    }

    public override void FixedUpdate()
    {
        player.rb.linearVelocityX = Mathf.MoveTowards(
        player.rb.linearVelocityX, 0f, player.deceleration * Time.fixedDeltaTime);
    }
}
