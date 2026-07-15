using UnityEngine;
using Unity.Cinemachine;

public class CameraConfinerRefresher : MonoBehaviour
{
    private CinemachineConfiner2D confiner;

    private void Awake()
    {
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    private void Start()
    {
        confiner.InvalidateBoundingShapeCache();
    }
}
