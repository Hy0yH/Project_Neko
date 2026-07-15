using UnityEngine;

public class SceneSpawnPoint : MonoBehaviour
{
    [SerializeField] private string spawnId; // 스폰 포인트의 ID

    public string SpawnId => spawnId; // 스폰 포인트의 ID를 반환하는 프로퍼티
}
