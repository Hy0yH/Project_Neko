using UnityEngine;
using UnityEngine.UI;

public class ChuruUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private Image[] churuSlots;
    [SerializeField] private Sprite churuEmpty;
    [SerializeField] private Sprite churuFull;
    [SerializeField] private Sprite[] churuFilling;

    private void OnEnable()
    {
        playerInventory.OnChuruChanged += UpdateUI;
    }
    private void OnDisable()
    {
        playerInventory.OnChuruChanged -= UpdateUI;
    }
    private void UpdateUI(int currentChuru, int maxChuru, int killsTowardNext)
    {
        for(int i = 0; i < churuSlots.Length; ++i)
        {
            if (i < currentChuru)
                churuSlots[i].sprite = churuFull;
            else if (i == currentChuru)
                churuSlots[i].sprite = (killsTowardNext == 0) ? churuEmpty : churuFilling[killsTowardNext - 1];
            else
                churuSlots[i].sprite = churuEmpty;
        }
    }
}
