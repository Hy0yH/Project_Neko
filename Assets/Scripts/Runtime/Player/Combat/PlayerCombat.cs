using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerForm playerForm;
    private PlayerAttackSO currentAttack;
    [SerializeField] private InputActionReference attackAction;
    private PlayerAudio playerAudio;
    private Animator anim;
    private enum AttackPhase { Idle, Charging, Startup, Active, Recovery }
    private AttackPhase currentPhase;
    private float stateTimer;
    private float chargeTimer;
    private HashSet<Collider2D> colliders = new HashSet<Collider2D>();

    [SerializeField] private GameObject chargeStartAura;
    [SerializeField] private GameObject chargeCompleteAura;

    private void Awake()
    {
        anim = GetComponent<Animator>();

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
        if (playerForm.IsTransforming) return;
        // 공격 입력 감지
        if (attackAction != null && attackAction.action.WasPressedThisFrame())
        {
            if(currentPhase == AttackPhase.Idle)
            {
                if (playerForm.CurrentForm.chargeAttack == null)
                    StartAttack(playerForm.CurrentForm.basicAttack);
                else
                    EnterCharging();
            }
        }
        if(currentPhase == AttackPhase.Charging)
        {
            chargeTimer += Time.deltaTime;

            // 오라 완료 전환
            bool isComplete = chargeTimer >= playerForm.CurrentForm.chargeAttack.chargeThreshold;
            chargeStartAura.SetActive(!isComplete);
            chargeCompleteAura.SetActive(isComplete);

            if(attackAction.action.WasReleasedThisFrame())
            {
                StopChargeEffect();
                if(chargeTimer >= playerForm.CurrentForm.chargeAttack.chargeThreshold)
                    StartAttack(playerForm.CurrentForm.chargeAttack);
                else
                    StartAttack(playerForm.CurrentForm.basicAttack);
            }
        }
        else if(currentPhase != AttackPhase.Idle)
        {
            stateTimer -= Time.deltaTime;
            if(stateTimer <= 0f)
            {
                switch(currentPhase)
                {
                    case AttackPhase.Startup:
                        currentPhase = AttackPhase.Active;
                        stateTimer = currentAttack.activeTime;

                        // 이펙트
                        Vector2 pos = GetHitboxWorldPosition();
                        if (currentAttack.hitEffectPrefab != null)
                        {
                            GameObject fx = Instantiate(currentAttack.hitEffectPrefab, transform);
                            fx.transform.localPosition = currentAttack.effectOffset;
                        }
                        break;
                    case AttackPhase.Active:
                        currentPhase = AttackPhase.Recovery;
                        stateTimer = currentAttack.recoveryTime;
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
    private void StartAttack(PlayerAttackSO pASO)
    {
        colliders.Clear();
        currentAttack = pASO;
        currentPhase = AttackPhase.Startup;
        stateTimer = currentAttack.startupTime;
        anim.SetTrigger(currentAttack.animatorTrigger);

        // 효과음
        playerAudio?.PlayScratch();

        Debug.Log($"Attack start {currentPhase}, {stateTimer}");
    }
    private void CheckHitbox()
    {
        Vector2 pos = GetHitboxWorldPosition();
        Collider2D[] cols = Physics2D.OverlapBoxAll(pos, currentAttack.hitboxSize, 0f, currentAttack.hitLayers);
        foreach(var col in cols)
        {
            if (colliders.Add(col))
            {
                IDamageable damageable = col.GetComponent<IDamageable>();
                damageable?.TakeDamage(currentAttack.damage);
                Debug.Log($"Hit {col.name} for {currentAttack.damage} damage");
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (currentAttack == null) return;
        Gizmos.color = currentPhase == AttackPhase.Active ? Color.red : Color.yellow;
        Vector2 pos = GetHitboxWorldPosition();
        Gizmos.DrawWireCube(pos, currentAttack.hitboxSize);
    }
    private Vector2 GetHitboxWorldPosition()
    {
        return transform.TransformPoint(currentAttack.hitboxOffset);
    }
    private void HandleDeath()
    {
        enabled = false;
    }
    private void EnterCharging()
    {
        chargeTimer = 0f;
        currentPhase = AttackPhase.Charging;
        anim.SetTrigger("AttackReady");
    }
    public void OnTransformEnd()
    {
        currentPhase = AttackPhase.Idle;
    }
    private void StopChargeEffect()
    {
        chargeCompleteAura.SetActive(false);
        chargeStartAura.SetActive(false);
    }

    public void SetCombatLocked(bool locked)
    {
        if (locked)
        {
            currentPhase = AttackPhase.Idle;
            stateTimer = 0f;
            
            if (chargeStartAura != null) chargeStartAura.SetActive(false);
            if (chargeCompleteAura != null) chargeCompleteAura.SetActive(false);
        }

        enabled = !locked;
    }
}