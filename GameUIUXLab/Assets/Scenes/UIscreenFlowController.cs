using UnityEngine;

public class UIScreenFlowController : MonoBehaviour
{
    private enum ScreenState
    {
        Title,
        Play,
        Pause,
        Result
    }

    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject playHudScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject resultScreen;

    private ScreenState currentState;

    void Start()
    {
        ShowTitle();
    }

    public void ShowTitle()
    {
        ChangeScreen(ScreenState.Title);
        Time.timeScale = 1f;
    }

    public void StartGame()
    {

        ChangeScreen(ScreenState.Play);
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        ChangeScreen(ScreenState.Pause);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        ChangeScreen(ScreenState.Play);
        Time.timeScale = 1f;
    }

    public void ShowResult()
    {
        ChangeScreen(ScreenState.Result);
        Time.timeScale = 1f;
    }

    private void ChangeScreen(ScreenState nextState)
    {
        currentState = nextState;

        titleScreen.SetActive(currentState == ScreenState.Title);
        playHudScreen.SetActive(currentState == ScreenState.Play);
        pauseScreen.SetActive(currentState == ScreenState.Pause);
        resultScreen.SetActive(currentState == ScreenState.Result);
    }
}