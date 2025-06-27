using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsManager2 : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject promptSair;
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Slider musicSlider;
    public Slider soundSlider;
    public Slider voiceSlider;
    public Text musicText;
    public Text soundText;
    public Text voiceText;
    public AudioSource musicSource;
    [SerializeField] public AudioSource[] voiceSources;
    //public AudioSource voiceSource2;
    //public AudioSource voiceSource3;
    //public AudioSource voiceSource4;
    [SerializeField] public AudioSource[] soundSources;
    //public AudioSource soundSource2;
    //public AudioSource soundSource3;

    private const string MusicVolumeKey = "MusicVolume";
    private const string VoiceVolumeKey = "VoiceVolume";
    private const string SoundVolumeKey = "SoundVolume";

    private Resolution[] availableResolutions;
    private Resolution snapshotResolution;
    private Resolution lastSelectedResolution;

    private bool snapshotFullscreen;
    private bool settingsPanelOpen = false;

    private int checkboxInteractionCount = 0;
    private int fullscreenChangeCount = 0;

    //private bool lastKnownFullscreenState;

    void Start()
    {
        lastKnownFullscreen = Screen.fullScreen;
        float savedMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 100f);
        float savedVoiceVolume = PlayerPrefs.GetFloat(VoiceVolumeKey, 100f);
        float savedSoundVolume = PlayerPrefs.GetFloat(SoundVolumeKey, 100f);
        voiceSlider.value = savedVoiceVolume;
        musicSlider.value = savedMusicVolume;
        soundSlider.value = savedSoundVolume;
        UpdateMusicValue(savedMusicVolume);
        musicSlider.onValueChanged.AddListener(UpdateMusicValue);
        UpdateSoundValue(soundSlider.value);
        soundSlider.onValueChanged.AddListener(UpdateSoundValue);
        UpdateVoiceValue(voiceSlider.value);
        voiceSlider.onValueChanged.AddListener(UpdateVoiceValue);
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);
        }
        else
        {
            Debug.LogWarning("fullscreenToggle is not assigned in the Inspector!");
        }
    }

    void UpdateMusicValue(float value)
    {
        musicSource.volume = value / 100f;

        musicText.text = value.ToString("0") + "%";

        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        PlayerPrefs.Save();
    }

    void UpdateSoundValue(float value)
    {
        foreach(var ss in soundSources)
        {
            ss.volume = value / 100f;
        }
        //soundSource.volume = value / 100f;
        //soundSource2.volume = value / 100f;
        //soundSource3.volume = value / 100f;

        soundText.text = value.ToString("0") + "%";

        PlayerPrefs.SetFloat(SoundVolumeKey, value);
        PlayerPrefs.Save();
    }

    void UpdateVoiceValue(float value)
    {
        foreach(var vs in voiceSources)
        {
            vs.volume = value / 100f;
        }
        //voiceSource1.volume = value / 100f;
        //voiceSource2.volume = value / 100f;
        //voiceSource3.volume = value / 100f;
        //voiceSource4.volume = value / 100f;

        voiceText.text = value.ToString("0") + "%";

        PlayerPrefs.SetFloat(VoiceVolumeKey, value);
        PlayerPrefs.Save();
    }

    void OnValidate()
    {
        if (fullscreenToggle == null)
            Debug.LogWarning("⚠️ fullscreenToggle is not assigned on SettingsManager", this);
        if (resolutionDropdown == null)
            Debug.LogWarning("⚠️ resolutionDropdown is not assigned on SettingsManager", this);
    }

    void Awake()
    {
        // Optional: load saved preference on start
        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        Screen.fullScreen = savedFullscreen;
    }

    private bool lastKnownFullscreen;



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel.activeSelf)
            {
                CloseSettingsPanel();
            }
            else if (!promptSair.activeSelf)
            {
                promptSair.SetActive(true);
            }

            //promptSair.SetActive(false);
        }

        //if (!settingsPanelOpen) return;

        /*if (Screen.fullScreen != fullscreenToggle.isOn)
        {
            // Sync toggle visually
            fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggleChanged);
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);
        }

        fullscreenChangeCount++;
        PlayerPrefs.Save();

        if ((checkboxInteractionCount == 0 && fullscreenChangeCount > 0) ||
            (checkboxInteractionCount > 0 && fullscreenChangeCount > 1))
        {
            snapshotFullscreen = Screen.fullScreen;
        }*/

        if (Screen.fullScreen != lastKnownFullscreen)
        {
            lastKnownFullscreen = Screen.fullScreen;

            // Update toggle UI safely
            fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggleChanged);
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);

            // Save to PlayerPrefs to persist
            PlayerPrefs.SetInt("Fullscreen", Screen.fullScreen ? 1 : 0);
            PlayerPrefs.Save();

            // Optional: Update snapshot if you're still using it for "Cancel"
            snapshotFullscreen = Screen.fullScreen;
        }

        /*if (!settingsPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            promptSair.SetActive(true);
        }*/
    }

    public void OnSettingsPanelOpened()
    {
        settingsPanelOpen = true;

        snapshotFullscreen = Screen.fullScreen;
        checkboxInteractionCount = 0;
        fullscreenChangeCount = 0;

        fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggleChanged);
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);

        PopulateResolutionDropdown();

        resolutionDropdown.onValueChanged.RemoveAllListeners();
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }

    public void OnSettingsPanelClosed()
    {
        settingsPanelOpen = false;
    }

    /*void OnEnable()
    {
        // When menu is opened, load stored settings
        //originalFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        // Apply to UI
        //fullscreenToggle.isOn = originalFullscreen;

        // Set pending state
        //pendingFullscreenState = originalFullscreen;

        //lastKnownFullscreenState = Screen.fullScreen;

        RefreshUIFromSystemSettings();

        // Listen to toggle changes
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);
    }*/

    /*void OnDisable()
    {
        // Clean up listeners to avoid duplicates
        fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggleChanged);
    }*/

    /*void RefreshUIFromSystemSettings()
    {
        bool currentFullscreen = Screen.fullScreen;

        // Set original and pending states to match real current fullscreen
        originalFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        pendingFullscreenState = currentFullscreen;
        lastKnownFullscreenState = currentFullscreen;

        // Update toggle without triggering listener
        fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggleChanged);
        fullscreenToggle.isOn = currentFullscreen;
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);
    }*/

    /*void Update()
    {
        if (Screen.fullScreen != lastKnownFullscreenState)
        {
            lastKnownFullscreenState = Screen.fullScreen;

            // Temporarily disable listener to avoid triggering logic
            fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggleChanged);
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);

            // Update pending state to reflect actual change
            pendingFullscreenState = Screen.fullScreen;
        }
    }*/

    //old one
    private void OnFullscreenToggleChanged(bool value)
    {
        if (!settingsPanelOpen) return;

        checkboxInteractionCount = Mathf.Clamp(checkboxInteractionCount + 1, 0, 1000);
        fullscreenChangeCount = 0;

        Screen.fullScreen = value;

        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    /*private System.Collections.IEnumerator ApplyFullscreenFromCheckbox(bool value)
    {
        checkboxInitiatedChange = true;
        checkboxUsed = true;

        Screen.fullScreen = value;

        // Wait a short frame so Update can recognize it
        yield return null;

        checkboxInitiatedChange = false;  // Reset AFTER Update has a chance to run
    }*/

    //new one
    /*public void OnFullscreenCheckboxToggled(bool value)
    {
        // Detect if this change came from external code or actual user input
        if (!settingsPanelOpen) return;

        Screen.fullScreen = value;
        lastFullscreenDuringSession = value;
        lastFullscreenSource = FullscreenChangeSource.Checkbox;
    }*/

    private void PopulateResolutionDropdown()
    {
        availableResolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        var options = new List<string>();
        List<Resolution> resolutionList = new List<Resolution>(availableResolutions);
        //int currentResolutionIndex = 0;
        Resolution currentResolution = new Resolution
        {
            width = Screen.width,
            height = Screen.height,
            refreshRate = Screen.currentResolution.refreshRate
        };

        int currentIndex = 0;

        bool exists = resolutionList.Exists(r =>
        r.width == currentResolution.width &&
        r.height == currentResolution.height &&
        r.refreshRate == currentResolution.refreshRate);

        if (!exists)
        {
            resolutionList.Insert(0, currentResolution);
            options.Add($"{currentResolution.width}×{currentResolution.height} @ {currentResolution.refreshRate}Hz");
            currentIndex = 0;
        }

        for (int i = exists ? 0 : 1; i < resolutionList.Count; i++)
        {
            Resolution res = resolutionList[i];
            string option = res.width + "×" + res.height + " @ " + res.refreshRate + "Hz";
            options.Add(option);

            if (res.width == Screen.width &&
                res.height == Screen.height &&
                res.refreshRate == Screen.currentResolution.refreshRate)
            {
                currentIndex = i;
            }
        }

        availableResolutions = resolutionList.ToArray();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();

        // Save the snapshot
        //snapshotResolution = availableResolutions[currentResolutionIndex];
        //lastSelectedResolution = snapshotResolution;
    }

    private void OnResolutionChanged(int index)
    {
        lastSelectedResolution = availableResolutions[index];
        ApplyResolution(lastSelectedResolution);
    }

    private void ApplyResolution(Resolution resolution)
    {
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate);
    }

    public void ApplySettings()
    {
        PlayerPrefs.SetInt("Fullscreen", Screen.fullScreen ? 1 : 0);
        PlayerPrefs.SetInt("ResolutionWidth", lastSelectedResolution.width);
        PlayerPrefs.SetInt("ResolutionHeight", lastSelectedResolution.height);
        PlayerPrefs.SetInt("RefreshRate", lastSelectedResolution.refreshRate);
        PlayerPrefs.Save();

        OnSettingsPanelClosed();
        ResetCounters();
    }

    public void CancelSettings()
    {
        Screen.fullScreen = snapshotFullscreen;

        fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggleChanged);
        fullscreenToggle.isOn = snapshotFullscreen;
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);

        ApplyResolution(snapshotResolution);
        int revertIndex = System.Array.IndexOf(availableResolutions, snapshotResolution);
        if (revertIndex >= 0)
        {
            resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
            resolutionDropdown.value = revertIndex;
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }

        OnSettingsPanelClosed();
        ResetCounters();
    }

    private void ResetCounters()
    {
        checkboxInteractionCount = 0;
        fullscreenChangeCount = 0;
    }

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
        promptSair.SetActive(false);
    }
}