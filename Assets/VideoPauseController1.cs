using UnityEngine;
using UnityEngine.Video;

public class VideoPauseController1 : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject[] panelsToCheck;
    [SerializeField] private GameManager gameManager;

    private void Update()
    {
        if (gameManager == null || videoPlayer == null || panelsToCheck == null)
            return;

        bool anyPanelActive = false;

        foreach (var panel in panelsToCheck)
        {
            if (panel != null && panel.activeSelf)
            {
                anyPanelActive = true;
                break;
            }
        }

        if (anyPanelActive)
        {
            if (videoPlayer.isPlaying)
                videoPlayer.Pause();
        }
        else
        {
            if (gameManager.gameComplete == true && !videoPlayer.isPlaying)
                videoPlayer.Play();
        }
    }
}
