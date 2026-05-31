using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;



public class MenuButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text targetText;

    [Header("Colors")]
    [SerializeField] private Color hoverColor = new Color(1f, 0.9f, 0.55f);

    [Header("Font Size")]
    [SerializeField] private float hoverFontSizeIncrease = 5f;

    private Color baseColor;
    private float originalFontSize;
    private bool isHovering;


    public void SetBaseColor(Color color)
    {
        baseColor = color;

        if (!isHovering && targetText != null)
        {
            targetText.color = baseColor;
        }
    }

    private void Awake()
    {
        if (targetText == null)
        {
            targetText = GetComponentInChildren<TMP_Text>();
        }

        if (targetText != null)
        {
            baseColor = targetText.color;
            originalFontSize = targetText.fontSize;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetText == null)
        {
            return;
        }

        originalFontSize = targetText.fontSize;
        isHovering = true;

        targetText.color = hoverColor;
        targetText.fontSize = originalFontSize + hoverFontSizeIncrease;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetHoverState();
    }

    public void OnDisable()
    {
        ResetHoverState();
    }

    private void ResetHoverState()
    {
        if (targetText == null)
        {
            return;
        }

        targetText.color = baseColor;
        targetText.fontSize = originalFontSize;
        isHovering = false;
    }
}
