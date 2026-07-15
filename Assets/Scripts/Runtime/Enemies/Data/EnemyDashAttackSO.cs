using UnityEngine;

[CreateAssetMenu(fileName = "NewDashAttack", menuName = "Enemy AI/Dash Attack")]
public class EnemyDashAttackSO : EnemyAttackSO
{
    [Header("Dash Settings")]
    public float dashSpeed; //돌진 속도

    public override void ExcuteAttack(Enemy enemy)
    {
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();

        if (rigid == null) return;
        if (enemy.playerTarget == null) return;

        float dirX = enemy.attackDirectionX; //거리 차이를 이용해 만든 방향값
        
        rigid.linearVelocity = new Vector2(dirX * dashSpeed, rigid.linearVelocity.y);
    }
}
