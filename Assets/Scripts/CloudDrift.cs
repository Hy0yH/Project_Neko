using UnityEngine;

public class CloudDrift : MonoBehaviour
{
    [SerializeField] private float moveSpeed; // 구름 이동 속도
    [SerializeField] private float verticalAmount; // 구름의 수직 이동 범위
    [SerializeField] private float verticalSpeed; // 구름의 수직 이동 속도

    private Vector3 startlocalPosition; // 구름의 초기 위치

    private void Start()
    {
        startlocalPosition = transform.localPosition; // 구름의 초기 위치 저장
    }

    private void Update()
    {
        float x = Time.time * moveSpeed; // 시간에 따라 x 좌표 계산
        float y = Mathf.Sin(Time.time * verticalSpeed) * verticalAmount; // 시간에 따라 y 좌표 계산

        transform.localEulerAngles = startlocalPosition + new Vector3(x, y, 0f); // 구름의 위치 업데이트
    }
}
