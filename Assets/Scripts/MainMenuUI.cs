using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private string startSceneName = "test";

    void Start()
    {
        ShowMainPanel();
    }

    public void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
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
        SceneManager.LoadScene("test");
    }
}
