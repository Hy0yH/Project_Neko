using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance; // 싱글톤

    private Vector3 respawnPoint;
    private string respawnScene;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GameObject player = GameObject.Find("SpawnPoint");
        SetRespawnPoint(player.transform.position);
    }

    public void SetRespawnPoint(Vector3 pos) { 
        respawnPoint = pos;
        respawnScene = SceneManager.GetActiveScene().name;
    }
    public Vector3 GetRespawnPoint() { return respawnPoint; }
    public string GetRespawnScene() { return respawnScene; }
}
