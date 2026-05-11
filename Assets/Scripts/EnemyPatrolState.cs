using Unity.VisualScripting;
using UnityEngine;

public class EnemyPatrolState : IEnemyState
{
    private Enemy enemy;

    public EnemyPatrolState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void EnterState()
    {
        
    }

    public void UpdateState()
    {
        
    }

    public void FixedUpdateState()
    {
        if(enemy.movementSO != null)
        {
            enemy.movementSO.ExcuteMove(enemy);
        }
    }

    public void ExitState()
    {
        
    }
}
