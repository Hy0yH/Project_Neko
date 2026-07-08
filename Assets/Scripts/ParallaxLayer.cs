using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float parallaxFactor = 0.1f;

    private Vector3 startPosition;
    private Vector3 cameraStartPosition;

    private void Start()
    {
        startPosition = transform.position;

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        cameraStartPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 cameraDelta = cameraTransform.position - cameraStartPosition;

        transform.position = startPosition + new Vector3(cameraDelta.x * parallaxFactor, cameraDelta.y * parallaxFactor, 0f);
    }
}
