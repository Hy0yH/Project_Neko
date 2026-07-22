using UnityEngine;

public class VillaFloorBlurController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Material blurMaterial;

    [Header("Floor Detection")]
    [SerializeField] private VillaFloorBlurZone startingZone;
    [SerializeField] private VillaFloorBlurZone[] floorZones;

    [SerializeField]
    private float heightSampleOffset = 0.2f;

    [SerializeField, Min(0f)]
    private float switchHysteresis = 0.1f;

    [Header("Blur")]
    [SerializeField, Range(0f, 12f)] private float blurRadius = 5f;
    [SerializeField, Range(0f, 0.1f)] private float feather = 0.02f;
    [SerializeField, Range(0f, 1f)] private float effectStrength = 1f;

    [Header("Cinematic")]
    [SerializeField, Min(0f)] private float cinematicFadeDuration = 0.5f;

    private bool isCinematicReveal;
    private float displayedEffectStrength;

    [SerializeField] private VillaFloorBlurZone currentZone;

    private static readonly int FocusBottomId =
        Shader.PropertyToID("_FocusBottom");

    private static readonly int FocusTopId =
        Shader.PropertyToID("_FocusTop");

    private static readonly int BlurRadiusId =
        Shader.PropertyToID("_BlurRadius");

    private static readonly int FeatherId =
        Shader.PropertyToID("_Feather");

    private static readonly int EffectStrengthId =
        Shader.PropertyToID("_EffectStrength");

    private void Start()
    {
        currentZone = startingZone;
        displayedEffectStrength = effectStrength;
        ResolveReferences();
    }

    public void SetCinematicReveal(bool reveal)
    {
        isCinematicReveal = reveal;
    }

    private void LateUpdate()
    {
        ResolveReferences();

        if (player == null || targetCamera == null || blurMaterial == null)
            return;

        UpdateCurrentFloor();
        UpdateBlurMaterial();
    }

    private void ResolveReferences()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.GetComponent<PlayerController>();
        }

        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void UpdateCurrentFloor()
    {
        if (floorZones == null || floorZones.Length == 0)
            return;

        float sampleY =
            player.transform.position.y + heightSampleOffset;

        VillaFloorBlurZone nearestZone = null;
        float nearestDistance = float.MaxValue;

        foreach (VillaFloorBlurZone zone in floorZones)
        {
            if (zone == null || !zone.IsValid)
                continue;

        float distance = Mathf.Abs(sampleY - zone.FocusY);

        if (distance < nearestDistance)
            {
            nearestDistance = distance;
            nearestZone = zone;
            }
        }

        if (nearestZone == null)
            return;

        if (currentZone == null || !currentZone.IsValid)
        {
            currentZone = nearestZone;
            return;
        }

        if (nearestZone == currentZone)
            return;

        float currentDistance =
            Mathf.Abs(sampleY - currentZone.FocusY);

        if (nearestDistance + switchHysteresis < currentDistance)
            currentZone = nearestZone;
    }

    private void UpdateBlurMaterial()
    {
        if (currentZone == null || !currentZone.IsValid)
        {
            blurMaterial.SetFloat(EffectStrengthId, 0f);
            return;
        }

        Vector3 bottomViewport = targetCamera.WorldToViewportPoint(
            new Vector3(
                targetCamera.transform.position.x,
                currentZone.BottomY,
                0f
            )
        );

        Vector3 topViewport = targetCamera.WorldToViewportPoint(
            new Vector3(
                targetCamera.transform.position.x,
                currentZone.TopY,
                0f
            )
        );

        float bottom = bottomViewport.y;
        float top = topViewport.y;

        if (bottom > top)
            (bottom, top) = (top, bottom);

        blurMaterial.SetFloat(FocusBottomId, bottom);
        blurMaterial.SetFloat(FocusTopId, top);
        blurMaterial.SetFloat(BlurRadiusId, blurRadius);
        blurMaterial.SetFloat(FeatherId, feather);
        
        float targetStrength = isCinematicReveal ? 0f : effectStrength;

        if (cinematicFadeDuration <= 0f)
        {
            displayedEffectStrength = targetStrength;
        }
        else
        {
            displayedEffectStrength = Mathf.MoveTowards(
                displayedEffectStrength,
                targetStrength,
                Time.deltaTime / cinematicFadeDuration
            );
        }

        blurMaterial.SetFloat(EffectStrengthId, displayedEffectStrength);
    }

    private void OnDisable()
    {
        if (blurMaterial != null)
            blurMaterial.SetFloat(EffectStrengthId, 0f);
    }
}