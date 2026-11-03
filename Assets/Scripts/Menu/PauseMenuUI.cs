
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject returnConfirmationPanel;
    [SerializeField] private GameObject exitConfirmationPanel;

    [Header("Scene")]
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private string mainMenuSceneName;

    private void Start()
    {
        Time.timeScale = 1f;

        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
        returnConfirmationPanel.SetActive(false);
        exitConfirmationPanel.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
        returnConfirmationPanel.SetActive(false);
        exitConfirmationPanel.SetActive(false);
    }

    public void OpenOptions()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ShowReturnConfirmation()
    {
        returnConfirmationPanel.SetActive(true);
    }

    public void CancelReturn()
    {
        returnConfirmationPanel.SetActive(false);
    }

    public void ConfirmReturn()
    {
        Time.timeScale = 1f;
        sceneLoader.LoadScene(mainMenuSceneName);
    }

    public void ShowExitConfirmation()
    {
        exitConfirmationPanel.SetActive(true);
    }

    public void CancelExit()
    {
        exitConfirmationPanel.SetActive(false);
    }

    public void ConfirmExit()
    {
        Time.timeScale = 1f;
        sceneLoader.Quit();
    }
}
