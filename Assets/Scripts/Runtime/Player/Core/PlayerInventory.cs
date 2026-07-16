using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private InputActionReference healAction;

    [Header("Churu settings")]
    [SerializeField] private int maxChuru = 3;
    [SerializeField] private int killsPerChuru = 5;
    [SerializeField] private int healAmount = 3;

    private int churuCount;
    private int killsTowardNextChuru;

    public event Action<int, int, int> OnChuruChanged;

    private void Start()
    {
        churuCount = maxChuru;
        killsTowardNextChuru = 0;
        OnChuruChanged?.Invoke(churuCount, maxChuru, killsTowardNextChuru);
    }

    private void OnEnable()
    {
        if (healAction != null) healAction.action.Enable();
        Enemy.OnEnemyDied += OnEnemyKilled;
        PlayerHealth.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        if (healAction != null) healAction.action.Disable();
        Enemy.OnEnemyDied -= OnEnemyKilled;
        PlayerHealth.OnDeath -= HandleDeath;
    }

    private void Update()
    {
        if(healAction != null && healAction.action.WasPressedThisFrame())
        {
            UseChuru();
        }
    }

    private void UseChuru()
    {
        if (churuCount == 0) return;
        if(!playerHealth.Heal(healAmount)) return;
        churuCount--;
        OnChuruChanged?.Invoke(churuCount, maxChuru, killsTowardNextChuru);
    }

    private void OnEnemyKilled(Enemy enemy)
    {
        if (churuCount == maxChuru) return;
        killsTowardNextChuru++;
        if(killsTowardNextChuru >= killsPerChuru)
        {
            churuCount++;
            killsTowardNextChuru = 0;
        }
        OnChuruChanged?.Invoke(churuCount, maxChuru, killsTowardNextChuru);
    }
    private void HandleDeath()
    {
        enabled = false;
    }
}