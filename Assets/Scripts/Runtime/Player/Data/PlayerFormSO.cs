using UnityEngine;

[CreateAssetMenu(fileName = "New Form", menuName = "ScriptableObjects/Form")]
public class PlayerFormSO : ScriptableObject
{
    [Header("Identity")]
    public string formName;

    [Header("Movement")]
    public float maxMoveSpeed;

    [Header("Animation")]
    public RuntimeAnimatorController animatorController;

    [Header("Attacks")]
    public PlayerAttackSO basicAttack;
    public PlayerAttackSO chargeAttack;

    [Header("Collider")]
    public Vector2 colliderSize;
    public Vector2 colliderOffset;
}
