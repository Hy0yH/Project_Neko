using UnityEngine;

public class VillaFloorBlurZone : MonoBehaviour
{
    [SerializeField] private Transform focusPoint;
    [SerializeField, Min(0.1f)] private float halfHeight = 2.9f;

    public bool IsValid => focusPoint != null;

    public float FocusY =>
        focusPoint != null
            ? focusPoint.position.y
            : transform.position.y;

    public float BottomY =>
        focusPoint != null
            ? focusPoint.position.y - halfHeight
            : transform.position.y;

    public float TopY =>
        focusPoint != null
            ? focusPoint.position.y + halfHeight
            : transform.position.y;

}