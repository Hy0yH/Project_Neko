using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "ScriptableObjects/Attack")]
public class PlayerAttackSO : ScriptableObject
{
    [Header("Damage")]
    public int damage = 1;

    [Header("Timing (sec)")]
    public float startupTime = 0.1f; // 모션 시작 ~ 판정 발생
    public float activeTime = 0.1f; // 히트박스 활성
    public float recoveryTime = 0.2f; // 판정 끝 ~ 다음 공격 가능
    public float chargeThreshold;
    // 총 쿨다운 = startup + active + recovery

    [Header("Hitbox")]
    public Vector2 hitboxOffset = new Vector2(0.8f, 0.5f);
    public Vector2 hitboxSize = new Vector2(1f, 1f);
    public LayerMask hitLayers;

    [Header("Animation")]
    public string animatorTrigger = "Attack";

    [Header("Effect")]
    public GameObject hitEffectPrefab;
    public Vector2 effectOffset;
}
