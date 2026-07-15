using UnityEngine;

[CreateAssetMenu(fileName = "NewBasicMovement", menuName = "Enemy AI/Basic Movement")]
public class EnemyBasicMovement : EnemyMovementSO
{
    public override void ExcuteMove(Enemy enemy)
    {
        if (enemy.rigid != null)
        {
            float direction = Mathf.Sign(enemy.transform.localScale.x); //적의 현재 방향을 얻기 위함. 양수면 무조건 1, 음수면 무조건 -1을 반환한다.
            enemy.rigid.linearVelocity = new Vector2(direction * moveSpeed, enemy.rigid.linearVelocity.y); //적의 X 속도 = 방향 * 속도, Y 속도는 유지
        }
    }
    
}
