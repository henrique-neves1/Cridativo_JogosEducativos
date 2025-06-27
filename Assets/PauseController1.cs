using UnityEngine;

public class PauseController1 : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject returnToMainMenuPrompt;
    public GameObject returnToDifficultySelectionPrompt;
    public GameObject settingsText;
    public GameObject mainMenuText;
    public GameObject resumeText;

    void Update()
    {
        bool isPauseActive = pausePanel != null && pausePanel.activeSelf;
        bool isSettingsActive = settingsPanel != null && settingsPanel.activeSelf;
        bool isReturnPromptActive = returnToMainMenuPrompt != null && returnToMainMenuPrompt.activeSelf;
        bool isReturnToDifficultyActive = returnToDifficultySelectionPrompt != null && returnToDifficultySelectionPrompt.activeSelf;

        // Press P to toggle pause panel depending on active state of panels
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isPauseActive && !isSettingsActive && !isReturnPromptActive && !isReturnToDifficultyActive)
            {
                // No panels active → activate pause panel
                pausePanel.SetActive(true);
            }
            else
            {
                // At least one panel active → deactivate all
                if (pausePanel != null) pausePanel.SetActive(false);
                if (settingsPanel != null) settingsPanel.SetActive(false);
                if (returnToMainMenuPrompt != null) returnToMainMenuPrompt.SetActive(false);
                settingsText.SetActive(false);
                mainMenuText.SetActive(false);
                resumeText.SetActive(false);
            }
        }
        // Press Escape to activate pause panel if it's the only one inactive
        else if (!isPauseActive && !isReturnToDifficultyActive && Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(true);
        }

        if (isReturnPromptActive && Input.GetKeyDown(KeyCode.Escape))
        {
            returnToMainMenuPrompt.SetActive(false);
        }

        if(isPauseActive && Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(false);
            settingsText.SetActive(false);
            mainMenuText.SetActive(false);
            resumeText.SetActive(false);
        }
    }
}
