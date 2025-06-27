using UnityEngine;

public class AudioModeManager : MonoBehaviour
{
    public AudioSourceToggle audioToggle;

    public ButtonHoverVoice1 buttonHoverVoice;
    public HoverFollower hoverFollower;

    public AudioClip idleHoverVoice;
    public AudioClip playingHoverVoice;

    private bool lastAudioPlayingState = false;

    void Update()
    {
        bool isPlaying = audioToggle != null && audioToggle.IsAudioPlaying();

        if (isPlaying != lastAudioPlayingState)
        {
            UpdateHoverComponents(isPlaying);
            lastAudioPlayingState = isPlaying;
        }
    }

    private void UpdateHoverComponents(bool isPlaying)
    {
        if (buttonHoverVoice != null)
        {
            buttonHoverVoice.hoverVoice = isPlaying ? playingHoverVoice : idleHoverVoice;
            buttonHoverVoice.StopHoverSoundImmediately();
        }
    }
}
