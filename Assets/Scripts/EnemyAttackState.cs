using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private Enemy enemy;

    public EnemyAttackState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void EnterState()
    {
        Debug.Log("Attack State Enter");
    }

    public void UpdateState()
    {
        
    }

    public void FixedUpdateState()
    {
        
    }

    public void ExitState()
    {
        
    }
}
