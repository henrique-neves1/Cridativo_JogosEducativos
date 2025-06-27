using UnityEngine;
using UnityEngine.Video;

public class VideoPauseController2 : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject[] panelsToCheck;
    [SerializeField] private MazeEndController mazeEndController;

    private void Update()
    {
        if (mazeEndController == null || videoPlayer == null || panelsToCheck == null)
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
            if (mazeEndController.IsGameCompleted() && !videoPlayer.isPlaying)
                videoPlayer.Play();
        }
    }
}
