using UnityEngine;

public class PlayerState
{
    // Finite State Machine에서 사용할 상태 클래스
    protected PlayerController player;

    public PlayerState(PlayerController playerController)
    {
        this.player = playerController;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}
