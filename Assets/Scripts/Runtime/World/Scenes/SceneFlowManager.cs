using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class SceneFlowManager : MonoBehaviour
{
    public static SceneFlowManager Instance;
    private bool pendingRespawn;
    private bool pendingTransition;
    private string pendingSpawnId;
    

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
        else if (pendingTransition)
        {
            pendingTransition = false;
            
            bool placed = PlaceAtSceneSpawnPoint(pendingSpawnId);

            if (!placed)
            {
                PlaceAtDefaultSpawn(player);
            }

            pendingSpawnId = null;
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

    private bool PlaceAtSceneSpawnPoint(string spawnId)
    {
        SceneSpawnPoint[] spawnPoints = FindObjectsByType<SceneSpawnPoint>(FindObjectsSortMode.None);

        foreach (SceneSpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.SpawnId == spawnId)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");

                if (player != null)
                {
                    player.transform.position = spawnPoint.transform.position;
                    return true;
                }
            }
        }

        Debug.LogWarning($"SpawnPoint를 찾지 못했습니다. Spawn Id : {spawnId}");
        return false;
    }

    public void TransitionToScene(string targetScene, string targetSpawnId)
    {
        pendingSpawnId = targetSpawnId;
        pendingTransition = true;

        SceneManager.LoadScene(targetScene);
    }

    private void PlaceAtDefaultSpawn(GameObject Player)
    {
        GameObject spawn = GameObject.Find("SpawnPoint");

        if (spawn != null)
        {
            Player.transform.position = spawn.transform.position;
        }
    }
}
