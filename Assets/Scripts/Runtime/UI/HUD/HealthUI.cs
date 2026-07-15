using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject[] hpFills;

    private void OnEnable()
    {
        if (playerHealth == null) return;

        playerHealth.OnHealthChanged += UpdateUI; // 이벤트 구독

        // 현재 HP 직접 읽어서 한번 갱신
        UpdateUI(playerHealth.currentHealth, hpFills.Length);
    }

    private void OnDisable()
    {
        if (playerHealth == null) return;

        playerHealth.OnHealthChanged -= UpdateUI; // 해제
    }

    private void UpdateUI(int currentHealth, int maxHealth)
    {
        for(int i = 0; i < maxHealth; ++i)
        {
            hpFills[i].SetActive(i < currentHealth); // 현재 HP보다 작은 인덱스는 활성화, 나머지는 비활성화
        }
    }
}
