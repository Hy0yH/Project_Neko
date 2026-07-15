using UnityEngine;
using UnityEngine.UI;

public class ChuruUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private Image[] churuFills;

    private void OnEnable()
    {
        playerInventory.OnChuruChanged += UpdateUI;
    }
    private void OnDisable()
    {
        playerInventory.OnChuruChanged -= UpdateUI;
    }
    private void UpdateUI(int currentChuru, int maxChuru, float progress)
    {
        for(int i = 0; i < maxChuru; ++i)
        {
            if (i < currentChuru) churuFills[i].fillAmount = 1f;
            else if (i == currentChuru) churuFills[i].fillAmount = progress;
            else churuFills[i].fillAmount = 0f;
        }
    }
}
