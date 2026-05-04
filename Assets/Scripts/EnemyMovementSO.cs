using UnityEngine;

public abstract class EnemyMovementSO : ScriptableObject
{
    [Header("Movement Settings")]
    public float moveSpeed;

    public abstract void ExcuteMove(Enemy enemy, Transform target);
}