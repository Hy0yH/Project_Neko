using UnityEngine;

[CreateAssetMenu(fileName = "NewDashAttack", menuName = "Enemy AI/Dash Attack")]
public class EnemyDashAttackSO : EnemyAttackSO
{
    [Header("Dash Settings")]
    public float dashSpeed; //돌진 속도
    public float dashDuration; //돌진 지속 시간

    public override void ExcuteAttack(Enemy enemy)
    {
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        if (rigid != null)
        {
            Vector2 dashDirection = (enemy.playerTarget.position - enemy.transform.position).normalized; //대쉬 방향은 플레이어 플레이어 위치 -> 에너미가 지정한 타겟의 위치 - 에너미의 위치
            rigid.linearVelocity = dashDirection * dashSpeed;
        }
    }
}
