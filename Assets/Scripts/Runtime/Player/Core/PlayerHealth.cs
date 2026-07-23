using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{

    // 플레이어의 최대 체력
    public int maxHealth { get; private set; } = 5;
    private bool isDead = false;

    // 피격 시 무적
    private float invincibilityDuration = 1.5f; // 1.5초 동안 무적
    private float invincibilityTimer = 0f; // 무적 타이머
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    [SerializeField] private float blinkInterval = 0.1f;
    [SerializeField] private Color invincibleColor = Color.gray;
    private Coroutine blinkCoroutine;
    [SerializeField] private CinemachineImpulseSource impulseSource;

    // 오디오
    private PlayerAudio playerAudio;

    // 플레이어의 현재 체력 (get은 외부에서 읽을 수 있지만, set은 private으로 설정하여 외부에서 직접 변경할 수 없도록 함)
    public int currentHealth { get; private set; }

    // 체력 변경 시 이벤트
    public event Action<int> OnHealthChanged;
    public static event Action OnDeath;

    // 부활 이벤트
    public static event Action OnRespawn;
    [SerializeField] private float respawnDelay = 5f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
        playerAudio = GetComponent<PlayerAudio>();
    }
    private void Start()
    {
        // 게임 시작 시 플레이어의 체력을 최대 체력으로 초기화
        currentHealth = maxHealth;

        // 초기 체력 UI 업데이트
        OnHealthChanged?.Invoke(currentHealth);
    }

    // 플레이어가 피격 당할 시
    public void TakeDamage(int damageAmount)
    {
        // 이미 죽었거나 무적 타이머가 남아있으면
        if (isDead || invincibilityTimer > 0f) return;

        // 체력 감소
        currentHealth -= damageAmount;
        Debug.Log($"damage : {damageAmount}, currentHP : {currentHealth}");

        // 카메라 흔들림
        impulseSource?.GenerateImpulse();

        // 피격 효과음
        playerAudio?.PlayHurt();

        // 무적 타이머 시작
        invincibilityTimer = invincibilityDuration;

        // 체력 변경 이벤트 호출
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }

        // 색 변경을 통한 깜빡임 표현
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = StartCoroutine(InvincibilityBlink());
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Died");
        // 죽음 처리
        OnDeath?.Invoke();
        StartCoroutine(RespawnAfterDelay());
    }

    private void Update()
    {
        // 무적 타이머가 0보다 크면 감소
        if (invincibilityTimer > 0f) invincibilityTimer -= Time.deltaTime;
    }

    public bool Heal(int amount)
    {
        // 풀피 OR 죽었으면 리턴
        if (currentHealth == maxHealth || currentHealth <= 0) return false;

        // 체력 회복
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        Debug.Log($"Healed: {amount}, currentHP: {currentHealth}");

        // 체력 변경 이벤트 호출
        OnHealthChanged?.Invoke(currentHealth);

        return true;
    }

    private IEnumerator InvincibilityBlink()
    {
        while(invincibilityTimer > 0f)
        {
            // 회색으로
            spriteRenderer.color = invincibleColor;
            yield return new WaitForSeconds(blinkInterval);

            // 원색으로
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(blinkInterval);
        }

        // 안전 복구
        spriteRenderer.color = originalColor;
    }

    public void Revive(Vector3 pos)
    {
        isDead = false;
        currentHealth = maxHealth;
        transform.position = pos;
        OnHealthChanged?.Invoke(currentHealth);
        OnRespawn?.Invoke();
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        SceneFlowManager.Instance.Respawn();
    }
}
