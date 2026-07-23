using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsOverlay;
    [SerializeField] private CutsceneController cutsceneController;
    [SerializeField] private string startSceneName = "UndergroundStage";
    [SerializeField] private DisplaySettingsController displaySettingsController;
    [SerializeField] private InputActionReference backAction;

    private void OnEnable()
    {
        if (backAction == null)
            return;

        backAction.action.performed += HandleBackInput;
        backAction.action.Enable();
    }

    private void OnDisable()
    {
        if (backAction == null)
            return;

        backAction.action.performed -= HandleBackInput;
        backAction.action.Disable();
    }

    void Start()
    {
        ShowMainPanel();
    }

    private void HandleBackInput(InputAction.CallbackContext context)
    {
        if (creditsOverlay != null && creditsOverlay.activeSelf)
        {
            CloseCredits();
            return;
        }

        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            CancelSettings();
        }
    }

    public void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
        creditsOverlay.SetActive(false);
    }

    public void OpenCredits()
    {
        creditsOverlay.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsOverlay.SetActive(false);
    }

    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        ShowMainPanel();
    }

    public void CancelSettings()
    {
        if (displaySettingsController != null)
        {
            displaySettingsController.DiscardChanges();
        }

        ShowMainPanel();
    }

    public void StartGame()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsOverlay.SetActive(false);

        cutsceneController.Play();
    }

    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}
