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

    public event Action<int, int, float> OnChuruChanged;

    private void Start()
    {
        churuCount = maxChuru;
        killsTowardNextChuru = 0;
        OnChuruChanged(churuCount, maxChuru, 0f);
    }

    private void OnEnable()
    {
        if (healAction != null) healAction.action.Enable();
        Enemy.OnEnemyDied += OnEnemyKilled;
    }

    private void OnDisable()
    {
        if (healAction != null) healAction.action.Disable();
        Enemy.OnEnemyDied -= OnEnemyKilled;
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
        float progress = (float)killsTowardNextChuru / killsPerChuru;
        OnChuruChanged?.Invoke(churuCount, maxChuru, progress);
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
        float progress = (float)killsTowardNextChuru / killsPerChuru;
        OnChuruChanged?.Invoke(churuCount, maxChuru, progress);
    }
}