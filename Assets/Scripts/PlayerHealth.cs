using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour, IDamageable
{

    // 플레이어의 최대 체력
    [SerializeField] private int maxHealth = 5;

    // 피격 시 무적
    private float invincibilityDuration = 1.5f; // 1.5초 동안 무적
    private float invincibilityTimer = 0f; // 무적 타이머

    // 플레이어의 현재 체력 (get은 외부에서 읽을 수 있지만, set은 private으로 설정하여 외부에서 직접 변경할 수 없도록 함)
    public int currentHealth { get; private set; }

    private void Start()
    {
        // 게임 시작 시 플레이어의 체력을 최대 체력으로 초기화
        currentHealth = maxHealth;
    }

    // 플레이어가 피격 당할 시
    public void TakeDamage(int damageAmount)
    {
        // 이미 죽었거나 무적 타이머가 남아있으면
        if (currentHealth <= 0 || invincibilityTimer > 0f) return;

        // 체력 감소
        currentHealth -= damageAmount;
        Debug.Log($"damage : {damageAmount}, currentHP : {currentHealth}");

        if(currentHealth <= 0)
        {
            Die();
        }

        // 무적 타이머 시작
        invincibilityTimer = invincibilityDuration;
    }

    private void Die()
    {
        Debug.Log("Died");
        // 죽음 처리
    }

    // 테스트용 코드
    private void Update()
    {
        // 무적 타이머가 0보다 크면 감소
        if (invincibilityTimer > 0f) invincibilityTimer -= Time.deltaTime;

        // 키보드의 H 키를 누르면 1의 피해를 입음
        if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
        {
            TakeDamage(1);
        }
    }
}
