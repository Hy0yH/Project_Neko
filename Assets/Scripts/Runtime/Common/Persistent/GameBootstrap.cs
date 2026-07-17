using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private GameObject persistentRootPrefab;

    private void Awake()
    {
        if (PersistentObject.Instance == null)
            Instantiate(persistentRootPrefab);
    }
}
