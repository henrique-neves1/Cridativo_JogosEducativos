using UnityEngine;

public class MusicPauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject returnToMainMenuPrompt;
    public AudioSource backgroundMusic;

    private bool musicWasPaused = false;

    void Update()
    {
        bool anyPanelActive = pausePanel.activeSelf || settingsPanel.activeSelf || returnToMainMenuPrompt.activeSelf;

        if (anyPanelActive && backgroundMusic.isPlaying)
        {
            backgroundMusic.Pause();
            musicWasPaused = true;
        }
        else if (!anyPanelActive && musicWasPaused)
        {
            backgroundMusic.UnPause();
            musicWasPaused = false;
        }
    }
}
