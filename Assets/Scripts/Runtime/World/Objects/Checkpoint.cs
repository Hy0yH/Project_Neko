using UnityEngine;
using UnityEngine.InputSystem;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private float healInterval = 1f;
    private float healTimer;
    
    private bool playerInRange;
    private bool isInside;
    private bool wasUpLastFrame;
    private bool wasSideLastFrame;

    private GameObject player;
    private Animator anim;
    private PlayerHealth playerHealth;

    private void OnEnable()
    {
        anim = GetComponent<Animator>();
    }

    private void OnDisable()
    {

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerInRange = true;
            player = col.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player")) playerInRange = false;
    }

    private void Update()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        bool upNow = input.y > 0.5f;
        bool sideNow = Mathf.Abs(input.x) > 0.5f;
        bool freshUp = upNow && !wasUpLastFrame;
        bool freshSide = sideNow && !wasSideLastFrame;

        if(isInside)
        {
            healTimer -= Time.deltaTime;
            if(healTimer <= 0f)
            {
                playerHealth.Heal(1);
                healTimer = healInterval;
            }
        }

        if(!isInside)
        {
            if (playerInRange && freshUp) EnterBox();
        } else
        {
            if (freshUp || freshSide) ExitBox();
        }

        wasUpLastFrame = upNow;
        wasSideLastFrame = sideNow;
    }

    private void EnterBox()
    {
        isInside = true;
        healTimer = healInterval;
        playerHealth = player.GetComponent<PlayerHealth>();
        CheckpointManager.Instance.SetRespawnPoint(transform.position);

        player.GetComponent<SpriteRenderer>().enabled = false;
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<PlayerCombat>().enabled = false;
        moveAction.action.Enable();

        anim.SetTrigger("Close");
    }
    private void ExitBox()
    {
        isInside = false;
        player.GetComponent<SpriteRenderer>().enabled = true;
        player.GetComponent<PlayerController>().enabled = true;
        player.GetComponent<PlayerCombat>().enabled = true;

        anim.SetTrigger("Open");
    }
}
