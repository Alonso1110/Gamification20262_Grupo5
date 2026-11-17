using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject exitConfirmationPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Scene Loader")]
    [SerializeField] private SceneLoader sceneLoader;

    private void Start()
    {
        if (exitConfirmationPanel) exitConfirmationPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);
    }

    public void ShowExitConfirmation() => exitConfirmationPanel.SetActive(true);
    public void HideExitConfirmation() => exitConfirmationPanel.SetActive(false);
    public void ConfirmExit() => sceneLoader.Quit();

    public void OpenOptions() => optionsPanel.SetActive(true);
    public void CloseOptions()
    {
        UIWindowPopUp popUp = optionsPanel.GetComponent<UIWindowPopUp>();
        if (popUp != null) popUp.CloseWindow();
        else optionsPanel.SetActive(false);
    }

    public void OpenCredits() => creditsPanel.SetActive(true);
    public void CloseCredits()
    {
        UIWindowPopUp popUp = creditsPanel.GetComponent<UIWindowPopUp>();
        if (popUp != null) popUp.CloseWindow();
        else creditsPanel.SetActive(false);
    }
}