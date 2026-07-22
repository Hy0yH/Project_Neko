using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsOverlay;
    [SerializeField] private CutsceneController cutsceneController;
    [SerializeField] private string startSceneName = "UndergroundStage";

    void Start()
    {
        ShowMainPanel();
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

    public void StartGame()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);
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
