using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image healthImage;
    [SerializeField] private Sprite[] hpSprites;

    private void OnEnable()
    {
        if (playerHealth == null) return;

        playerHealth.OnHealthChanged += UpdateUI; // 이벤트 구독

        // 현재 HP 직접 읽어서 한번 갱신
        UpdateUI(playerHealth.currentHealth);
    }

    private void OnDisable()
    {
        if (playerHealth == null) return;

        playerHealth.OnHealthChanged -= UpdateUI; // 해제
    }

    private void UpdateUI(int currentHealth)
    {
        if(currentHealth <= 0)
        {
            healthImage.enabled = false;
            return;
        }
        healthImage.enabled = true;
        healthImage.sprite = hpSprites[currentHealth - 1];
    }
}
