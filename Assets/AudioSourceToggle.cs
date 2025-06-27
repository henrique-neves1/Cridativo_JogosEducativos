using UnityEngine;
using UnityEngine.UI;

public class AudioSourceToggle : MonoBehaviour
{
    public Image playingImage;  // Image to show when audio is playing
    public Image idleImage;     // Image to show when audio is not playing
    public GameObject playingFollower;
    public GameObject idleFollower;

    private AudioSource audioSource;
    private Button button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(ToggleAudio);

        UpdateImages();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateImages();
    }

    private void ToggleAudio()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
        else
            audioSource.Play();

        UpdateImages();
    }

    private void UpdateImages()
    {
        if (playingImage != null && idleImage != null && playingFollower != null && idleFollower != null)
        {
            bool isPlaying = audioSource.isPlaying;
            playingImage.gameObject.SetActive(isPlaying);
            playingFollower.gameObject.SetActive(isPlaying);
            idleImage.gameObject.SetActive(!isPlaying);
            idleFollower.gameObject.SetActive(!isPlaying);
        }
    }

    public bool IsAudioPlaying()
    {
        return audioSource != null && audioSource.isPlaying;
    }
}
