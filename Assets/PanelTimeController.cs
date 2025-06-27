using UnityEngine;
using UnityEngine.UI;

public class PanelTimeController : MonoBehaviour
{
    [Tooltip("The panel whose time will be paused/resumed.")]
    public GameObject targetPanel;
    [Tooltip("Panels that can cause time to stop if any is active.")]
    public GameObject[] pausePanels;

    private bool timeIsPaused = false;
    private AudioSource[] audioSources;

    void Start()
    {
        if (targetPanel != null)
        {
            //videoPlayers = targetPanel.GetComponentsInChildren<VideoPlayer>(true);
            audioSources = targetPanel.GetComponentsInChildren<AudioSource>(true);
        }
    }

    void Update()
    {
        bool shouldPause = false;

        // Check if any pausePanel is active
        foreach (GameObject panel in pausePanels)
        {
            if (panel != null && panel.activeInHierarchy)
            {
                shouldPause = true;
                break;
            }
        }

        // Pause or resume time accordingly
        if (shouldPause && !timeIsPaused)
        {
            PauseTime();
        }
        else if (!shouldPause && timeIsPaused)
        {
            ResumeTime();
        }
    }

    void PauseTime()
    {
        Time.timeScale = 0f;
        timeIsPaused = true;
        foreach (var audio in audioSources)
        {
            if (audio.isPlaying)
                audio.Pause();
        }
        Debug.Log("Time and audio paused.");
    }

    void ResumeTime()
    {
        Time.timeScale = 1f;
        timeIsPaused = false;

        foreach (var audio in audioSources)
        {
            if (audio != null && !audio.isPlaying)
                audio.UnPause(); // Resume from Pause
        }

        Debug.Log("Time and audio unpaused.");
    }
}
