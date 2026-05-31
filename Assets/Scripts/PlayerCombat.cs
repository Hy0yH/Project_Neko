using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerAttackSO attackData;
    [SerializeField] private InputActionReference attackAction;
    private PlayerAudio playerAudio;
    private Animator anim;
    private enum AttackPhase { Idle, Startup, Active, Recovery }
    private AttackPhase currentPhase;
    private float stateTimer;
    private HashSet<Collider2D> colliders = new HashSet<Collider2D>();

    private void Awake()
    {
        anim = GetComponent<Animator>();

        if (attackData == null) Debug.LogError("AttackSO is not assigned", this);

        playerAudio = GetComponent<PlayerAudio>();
    }
    private void OnEnable()
    {
        if (attackAction != null) attackAction.action.Enable();
        PlayerHealth.OnDeath += HandleDeath;
    }
    private void OnDisable()
    {
        if (attackAction != null) attackAction.action.Disable();
        PlayerHealth.OnDeath -= HandleDeath;
    }
    private void Update()
    {
        // 공격 입력 감지
        if (attackAction != null && attackAction.action.WasPressedThisFrame())
        {
            if(currentPhase == AttackPhase.Idle)
            {
                StartAttack();
            }
        }
        if(currentPhase != AttackPhase.Idle)
        {
            stateTimer -= Time.deltaTime;
            if(stateTimer <= 0f)
            {
                switch(currentPhase)
                {
                    case AttackPhase.Startup:
                        currentPhase = AttackPhase.Active;
                        stateTimer = attackData.activeTime;
                        break;
                    case AttackPhase.Active:
                        currentPhase = AttackPhase.Recovery;
                        stateTimer = attackData.recoveryTime;
                        break;
                    case AttackPhase.Recovery:
                        currentPhase = AttackPhase.Idle;
                        stateTimer = 0f;
                        break;
                }
            }
        }
        if (currentPhase == AttackPhase.Active) CheckHitbox();
    }
    private void StartAttack()
    {
        colliders.Clear();
        currentPhase = AttackPhase.Startup;
        stateTimer = attackData.startupTime;
        anim.SetTrigger(attackData.animatorTrigger);

        // 효과음
        playerAudio?.PlayScratch();

        Debug.Log("Attack start");
    }
    private void CheckHitbox()
    {
        Vector2 pos = GetHitboxWorldPosition();
        Collider2D[] cols = Physics2D.OverlapBoxAll(pos, attackData.hitboxSize, 0f, attackData.hitLayers);
        foreach(var col in cols)
        {
            if (colliders.Add(col))
            {
                IDamageable damageable = col.GetComponent<IDamageable>();
                damageable?.TakeDamage(attackData.damage);
                Debug.Log($"Hit {col.name} for {attackData.damage} damage");
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (attackData == null) return;
        Gizmos.color = currentPhase == AttackPhase.Active ? Color.red : Color.yellow;
        Vector2 pos = GetHitboxWorldPosition();
        Gizmos.DrawWireCube(pos, attackData.hitboxSize);
    }
    private Vector2 GetHitboxWorldPosition()
    {
        return transform.TransformPoint(attackData.hitboxOffset);
    }
    private void HandleDeath()
    {
        enabled = false;
    }
}
