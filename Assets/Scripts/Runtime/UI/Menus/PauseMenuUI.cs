using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pauseRoot;
    [SerializeField] private GameObject pauseButtons;
    [SerializeField] private GameObject settingsPanel;

    [Header("UI Selection")]
    [SerializeField] private GameObject firstSelectedButton;

    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputActionReference pauseAction;

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused;

    private void Awake()
    {
        isPaused = false;

        pauseRoot.SetActive(false);
        pauseButtons.SetActive(true);
        settingsPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        if (pauseAction == null)
            return;

        pauseAction.action.performed += HandlePauseInput;
        pauseAction.action.Enable();
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= HandlePauseInput;
            pauseAction.action.Disable();
        }

        RestoreGameState();
    }

    private void HandlePauseInput(InputAction.CallbackContext context)
    {
        // 설정 화면에서 ESC를 누르면 일시정지 첫 화면으로 돌아갑니다.
        if (isPaused && settingsPanel.activeSelf)
        {
            ShowPauseButtons();
            return;
        }

        SetPaused(!isPaused);
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;

        if (paused)
        {
            pauseRoot.SetActive(true);
            pauseButtons.SetActive(true);
            settingsPanel.SetActive(false);

            Time.timeScale = 0f;
            SetPlayerInputEnabled(false);

            SelectFirstButton();
        }
        else
        {
            pauseRoot.SetActive(false);

            Time.timeScale = 1f;
            SetPlayerInputEnabled(true);

            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OpenSettings()
    {
        if (!isPaused)
            return;

        pauseButtons.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ShowPauseButtons()
    {
        settingsPanel.SetActive(false);
        pauseButtons.SetActive(true);

        SelectFirstButton();
    }

    public void GoToMainMenu()
    {
        // Time.timeScale은 씬을 변경해도 자동 복구되지 않습니다.
        Time.timeScale = 1f;

        if (PersistentObject.Instance != null)
            Destroy(PersistentObject.Instance.gameObject);

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void SetPlayerInputEnabled(bool enabled)
    {
        if (playerInput == null || playerInput.actions == null)
            return;

        InputActionMap playerMap =
            playerInput.actions.FindActionMap("Player", false);

        if (playerMap == null)
            return;

        if (enabled)
            playerMap.Enable();
        else
            playerMap.Disable();
    }

    private void SelectFirstButton()
    {
        if (EventSystem.current == null || firstSelectedButton == null)
            return;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    private void RestoreGameState()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SetPlayerInputEnabled(true);
    }
}