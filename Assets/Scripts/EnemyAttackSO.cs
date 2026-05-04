using UnityEngine;

public abstract class EnemyAttackSO : ScriptableObject
{
    [Header("Attack Settings")]
    public float enemyDamage;
    public float enemyAttackRange;
    public float enemyAttackDelay;

    public abstract void ExcuteAttack(Enemy enemy);
}