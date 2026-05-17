using UnityEngine;

public abstract class EnemyAttackSO : ScriptableObject
{
    [Header("Attack Settings")]
    public float enemyDamage;
    public float enemyAttackRange;
    public float enemyAttackDelay;
    public float enemyAttackDuration;
    public float enemyRecoveryTime;

    public abstract void ExcuteAttack(Enemy enemy);
}