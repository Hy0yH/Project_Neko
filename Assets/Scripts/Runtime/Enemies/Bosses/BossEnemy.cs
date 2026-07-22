using System.Collections;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    private enum BossState
    {
        Idle,
        InfinityAttack,
        DashToPlayer
    }
    
    [Header("Target")]
    [SerializeField] private Transform playerTarget;

    [Header("Infinity Attack")]
    [SerializeField] private float loopDuration = 3f;
    [SerializeField] private float horizontalRadius = 2.5f;
    [SerializeField] private float verticalRadius = 1.2f;
    [SerializeField] private float attackRange = 3f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 6f;
    [SerializeField] private float dashStopDistance = 0.3f;
    [SerializeField] private float maxDashTime = 2f;

    [Header("Collision")]
    [SerializeField] private bool ignorePlayerCollision = true;

    [Header("Visual")]
    [SerializeField] private bool spriteFacingRightByDefault = false; //스프라이트가 기본적으로 오른쪽을 향하는지 여부

    [Header("Player Damage")]
    [SerializeField, Min(1)] private int contactDamage = 1;

    private bool isBattleActive;
    
    [HideInInspector] public Rigidbody2D rigid;
    private BossState currentState = BossState.Idle;

    private Vector2 lockedCenter;
    private Vector2 dashTargetPosition;
    private Vector2 dashInfinityCenter;
    private float dashEntryAngle;
    private float currentAngle;
    private float traveledAngle;
    private float dashTimer;
    private SpriteRenderer spriteRenderer;


    private void Start()
    {
        
        StopPattern();
    }
    
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case BossState.InfinityAttack:
                UpdateInfinityAttack();
                break;
            case BossState.DashToPlayer:
                UpdateDashToPlayer();
                break;
        }
    }

    private void UpdateFacingToPlayer(Vector2 bossPosition)
    {
        if (spriteRenderer == null) return;
        if (playerTarget == null) return;

        float directionX = playerTarget.position.x - bossPosition.x;

        if (Mathf.Approximately(directionX, 0f)) return;

        bool isPlayerOnRight = directionX > 0f;

        spriteRenderer.flipX = spriteFacingRightByDefault ? !isPlayerOnRight : isPlayerOnRight;

    }

    public void StartInfinityAttack()
    {
        if (currentState == BossState.InfinityAttack) return;
        if (currentState == BossState.DashToPlayer) return;

        EnterInfinityAttack();
    }

    private void EnterInfinityAttack()
    {
        if (playerTarget == null)
        {
            currentState = BossState.Idle;
            return;
        }
        
        IgnoreCollisionWithPlayer();

        lockedCenter = playerTarget.position;

        currentAngle = FindCloseInfinityAngle(lockedCenter, GetCurrentPosition());
        traveledAngle = 0f;

        currentState = BossState.InfinityAttack;
    }

    public void UpdateInfinityAttack()
    {
        float safeLoopDuration = Mathf.Max(loopDuration, 0.01f);
        float angleSpeed = Mathf.PI * 2f / safeLoopDuration;
        float deltaAngle = angleSpeed * Time.fixedDeltaTime;

        currentAngle += deltaAngle;
        traveledAngle += deltaAngle;

        MoveTo(GetInfinityPosition(currentAngle));

        if (traveledAngle >= Mathf.PI * 2f)
        {
            DecideNextState();
        }
    }

    private Vector2 GetInfinityPosition(float angle)
    {
        float x = Mathf.Sin(angle) * horizontalRadius;
        float y = Mathf.Sin(angle * 2f) * verticalRadius;

        return lockedCenter + new Vector2(x, y);

    }

    private Vector2 GetInfinityPosition(Vector2 center, float angle)
    {
        float x = Mathf.Sin(angle) * horizontalRadius;
        float y = Mathf.Sin(angle * 2f) * verticalRadius;

        return center + new Vector2(x, y);
    }

    public float FindCloseInfinityAngle(Vector2 center, Vector2 referencePosition)
    {
        float closestAngle = 0f;
        float closestDistance = float.MaxValue;

        int sampleCount = 64;

        for (int i = 0 ; i < sampleCount ; i++)
        {
            float angle = Mathf.PI * 2f * i / sampleCount;
            Vector2 samplePosition = GetInfinityPosition(center, angle);
            
            float distance = Vector2.SqrMagnitude(referencePosition - samplePosition);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestAngle = angle;
            }
        }

        return closestAngle;
    }

    private void DecideNextState()
    {
        if (playerTarget == null)
        {
            currentState = BossState.Idle;
            return;
        }

        float distanceFromLockedCenter = Vector2.Distance(lockedCenter, playerTarget.position);

        if (distanceFromLockedCenter > attackRange)
        {
            StartDashToPlayer();
        }
        else
        {
            EnterInfinityAttack();
        }


    }

   private void StartDashToPlayer()
    {
        if (playerTarget == null)
        {
            currentState = BossState.Idle;
            return;
        }

        IgnoreCollisionWithPlayer();

        Vector2 currentPosition = rigid != null ? rigid.position : (Vector2)transform.position;
        Vector2 playerPosition = playerTarget.position;

        dashInfinityCenter = playerPosition;
        dashEntryAngle = FindCloseInfinityAngle(dashInfinityCenter, currentPosition);
        dashTargetPosition = GetInfinityPosition(dashInfinityCenter, dashEntryAngle);

        dashTimer = 0f;
        currentState = BossState.DashToPlayer;
    }

    private void UpdateDashToPlayer()
    {
        dashTimer += Time.fixedDeltaTime;

        Vector2 currentPosition = rigid != null ? rigid.position : (Vector2)transform.position;

        if (Vector2.Distance(currentPosition, dashTargetPosition) <= dashStopDistance)
        {
            EnterInfinityAttackFromDash();
            return;
        }

        Vector2 nextPosition = Vector2.MoveTowards
        (
            currentPosition,
            dashTargetPosition,
            dashSpeed * Time.fixedDeltaTime
        );

        MoveTo(nextPosition);
    }

    private void MoveTo(Vector2 position)
    {
        UpdateFacingToPlayer(position);

        if (rigid != null)
        {
            rigid.linearVelocity = Vector2.zero;
            rigid.angularVelocity = 0f;
            rigid.MovePosition(position);
        }
        else
        {
            transform.position = position;
        }
    }

    private Vector2 GetCurrentPosition()
    {
        return rigid != null ? rigid.position : (Vector2)transform.position;
    }

    private void EnterInfinityAttackFromDash()
    {
        IgnoreCollisionWithPlayer();

        lockedCenter = dashInfinityCenter;
        currentAngle = dashEntryAngle;
        traveledAngle = 0f;

        currentState = BossState.InfinityAttack;
    }

    private void IgnoreCollisionWithPlayer()
    {
        if (!ignorePlayerCollision || playerTarget == null) return;

        Collider2D[] bossColliders = GetComponentsInChildren<Collider2D>();
        Collider2D[] playerColliders = playerTarget.root.GetComponentsInChildren<Collider2D>();
    
        foreach (Collider2D bossCollider in bossColliders)
        {
            foreach (Collider2D playerCollider in playerColliders)
            {
                if (bossCollider != null && playerCollider != null)
                {
                    Physics2D.IgnoreCollision(bossCollider, playerCollider, true);
                }
            }
        }
    }

    public void BeginBattle(Transform player)
    {
        playerTarget = player;
        
        currentState = BossState.Idle;
        isBattleActive = true;

        IgnoreCollisionWithPlayer();
        StartDashToPlayer();
    }

    public void StopPattern()
    {
        currentState = BossState.Idle;
        isBattleActive = false;

        if (rigid != null)
        {
            rigid.linearVelocity = Vector2.zero;
            rigid.angularVelocity = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void TryDamagePlayer(Collider2D other)
    {
        if (!isBattleActive)
            return;

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
            return;

        playerHealth.TakeDamage(contactDamage);
    }
}
