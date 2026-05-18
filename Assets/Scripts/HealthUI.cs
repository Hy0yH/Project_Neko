using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image[] hpSlots;
    [SerializeField] private Color fullColor = Color.green;
    [SerializeField] private Color emptyColor = Color.darkGray;

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void UpdateUI(int currentHealth, int maxHealth)
    {

    }
}
