using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string targetScene;
    [SerializeField] private string targetSpawnId;

    [Header("Directional Spawn")]
    [SerializeField] private bool selectSpawnByDirection;
    [SerializeField] private string leftSpawnId = "FromUnderground_Left";
    [SerializeField] private string rightSpawnId = "FromUnderground_Right";

    private bool isTransitioning;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTransitioning) return;
        
        if (!collision.CompareTag("Player")) return;


        string selectedSpawnId = targetSpawnId;

        if (selectSpawnByDirection)
        {
            Rigidbody2D rb = collision.attachedRigidbody;
            if (rb == null) return;

            if (rb.linearVelocityX < -0.1f)
            {
                selectedSpawnId = leftSpawnId;
            }
            else if (rb.linearVelocityX > 0.1f)
            {
                selectedSpawnId = rightSpawnId;
            }
        
            else
            {
                PlayerController player = rb.GetComponent<PlayerController>();

                if (player != null)
                {
                    selectedSpawnId = player.facingDirection < 0 ? leftSpawnId : rightSpawnId;
                }
            }
        }
        

        isTransitioning = true;
        SceneFlowManager.Instance.TransitionToScene(targetScene, selectedSpawnId);
    }
}
