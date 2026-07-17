using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string targetScene;
    [SerializeField] private string targetSpawnId;

    private bool isTransitioning;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTransitioning) return;
        
        if (!collision.CompareTag("Player")) return;

        isTransitioning = true;

        SceneFlowManager.Instance.TransitionToScene(targetScene, targetSpawnId);
    }
}
