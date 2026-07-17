using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance; // 싱글톤
    [SerializeField] private Vector2 respawnPoint = new Vector2(-43f, 3f);

    private void Awake()
    {
        Instance = this;
    }

    public void SetRespawnPoint(Vector3 pos) { respawnPoint = pos; }
    public Vector3 GetRespawnPoint() { return respawnPoint; }
}
