using System.Collections;
using UnityEngine;

public class BossHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField, Min(1)] private int maxHealth = 20;

    private int currentHealth;
    private bool isDead;

    [Header("Hit Flash")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Material hitFlashMaterial;
    [SerializeField, Min(0f)] private float hitFlashDuration = 0.1f;

    [Header("Death")]
    [SerializeField, Min(0f)] private float destroyDelay = 0.15f;

    [Header("Hit Sound")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip hitClip;
    [SerializeField, Range(0f, 1f)] private float hitVolume = 1f;

    private BossEnemy bossEnemy;
    private Material originalMaterial;
    private Coroutine hitFlashCoroutine;

    private void Awake()
    {
        bossEnemy = GetComponent<BossEnemy>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalMaterial = spriteRenderer.material;

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead || damageAmount <= 0)
            return;

        currentHealth = Mathf.Max(
            0,
            currentHealth - damageAmount
        );

        Debug.Log(
            $"Boss Damage: {damageAmount}, " +
            $"HP: {currentHealth}/{maxHealth}"
        );

        if (sfxSource != null && hitClip != null)
            sfxSource.PlayOneShot(hitClip, hitVolume);

        PlayHitFlash();

        if (currentHealth <= 0)
            Die();
    }

    private void PlayHitFlash()
    {
        if (spriteRenderer == null || hitFlashMaterial == null)
            return;

        if (hitFlashCoroutine != null)
            StopCoroutine(hitFlashCoroutine);

        hitFlashCoroutine = StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        spriteRenderer.material = hitFlashMaterial;

        yield return new WaitForSeconds(hitFlashDuration);

        if (spriteRenderer != null)
            spriteRenderer.material = originalMaterial;

        hitFlashCoroutine = null;
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // 보스 이동 및 공격 정지
        bossEnemy?.StopPattern();

        // 죽은 뒤 추가 피격 방지
        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D bossCollider in colliders)
        {
            bossCollider.enabled = false;
        }

        Destroy(gameObject, destroyDelay);
    }
}

