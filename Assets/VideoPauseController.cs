using UnityEngine;
using UnityEngine.Video;

public class VideoPauseController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject[] panelsToCheck;
    [SerializeField] private CartasController cartasController;

    private void Update()
    {
        if (cartasController == null || videoPlayer == null || panelsToCheck == null)
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
            if (cartasController.IsGameCompleted() && !videoPlayer.isPlaying)
                videoPlayer.Play();
        }
    }
}
