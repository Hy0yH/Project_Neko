using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class SceneFlowManager : MonoBehaviour
{
    public static SceneFlowManager Instance;
    private bool pendingRespawn;

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        if (pendingRespawn)
        {
            pendingRespawn = false;
            PlaceAtCheckpoint();
        }
        else
        {
            GameObject spawn = GameObject.Find("SpawnPoint");
            if (spawn != null) player.transform.position = spawn.transform.position;
        }

        CinemachineCamera cmCam = FindFirstObjectByType<CinemachineCamera>();
        if (cmCam != null)
        {
            cmCam.Follow = player.transform;
            cmCam.Lens.OrthographicSize = 5f;
        }
    }
    public void Respawn()
    {
        string targetScene = CheckpointManager.Instance.GetRespawnScene();
        if (targetScene == SceneManager.GetActiveScene().name)
        {
            PlaceAtCheckpoint();
        } else
        {
            pendingRespawn = true;
            SceneManager.LoadScene(targetScene);
        }
    }
    private void PlaceAtCheckpoint()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 pos = CheckpointManager.Instance.GetRespawnPoint();
        player.GetComponent<PlayerHealth>().Revive(pos);
    }
}
