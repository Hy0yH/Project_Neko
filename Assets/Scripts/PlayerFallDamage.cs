using UnityEngine;

public class PlayerFallDamage : MonoBehaviour
{
    private PlayerController playerController;
    private Rigidbody2D rb;
    private PlayerHealth playerHealth;
    [SerializeField] private float damageVelocityThreshold = -15f;
    [SerializeField] private int fallDamage = 3;

    private float lowestVelocityY;
    private bool wasGroundedLastFrame;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
    }
    private void Update()
    {
        if (!playerController.isGrounded)
            lowestVelocityY = Mathf.Min(lowestVelocityY, rb.linearVelocityY);
        else
        {
            if (!wasGroundedLastFrame && lowestVelocityY < damageVelocityThreshold)
                playerHealth.TakeDamage(fallDamage);

            lowestVelocityY = 0f;
        }

        wasGroundedLastFrame = playerController.isGrounded;
    }
}
