using UnityEngine;
using UnityEngine.UI;

public class SliderTextValueChange : MonoBehaviour
{
    public Slider musicSlider;
    public Slider soundSlider;
    public Slider voiceSlider;
    public Text musicText;
    public Text soundText;
    public Text voiceText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateMusicValue(musicSlider.value);
        musicSlider.onValueChanged.AddListener(UpdateMusicValue);
        UpdateSoundValue(soundSlider.value);
        soundSlider.onValueChanged.AddListener(UpdateSoundValue);
        UpdateVoiceValue(voiceSlider.value);
        voiceSlider.onValueChanged.AddListener(UpdateVoiceValue);
    }

    void UpdateMusicValue(float value)
    {
        musicText.text = value.ToString("0") + "%";
    }

    void UpdateSoundValue(float value)
    {
        soundText.text = value.ToString("0") + "%";
    }

    void UpdateVoiceValue(float value)
    {
        voiceText.text = value.ToString("0") + "%";
    }
}
