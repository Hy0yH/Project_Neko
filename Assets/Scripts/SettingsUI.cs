using UnityEngine;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private GameObject displayPanel;
    [SerializeField] private GameObject keyBindingPanel;

    [Header("Menu Texts")]
    [SerializeField] private TMP_Text displaySoundSettingsText;
    [SerializeField] private TMP_Text keyBindingSettingsText;

    [Header("Colors")]
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color normalColor = new Color(0.5f, 0.5f, 0.5f); 

    [SerializeField] private MenuButtonHoverEffect displaySettingsHoverEffect;
    [SerializeField] private MenuButtonHoverEffect keyBindingSettingsHoverEffect;


    void Start()
    {
        ShowDisplayPanel();
    }

    public void ShowDisplayPanel()
    {
        displayPanel.SetActive(true);
        keyBindingPanel.SetActive(false);

        displaySettingsHoverEffect.SetBaseColor(selectedColor);
        keyBindingSettingsHoverEffect.SetBaseColor(normalColor);
    }

    public void ShowKeyBindingPanel()
    {
        displayPanel.SetActive(false);
        keyBindingPanel.SetActive(true);

        displaySettingsHoverEffect.SetBaseColor(normalColor);
        keyBindingSettingsHoverEffect.SetBaseColor(selectedColor);
    }

}
