using UnityEngine;
using UnityEngine.UI;

public class ExitPrompt : MonoBehaviour
{
    public Button buttonSair;
    public GameObject promptSair;
    public GameObject settings;

    private void Start()
    {
        if (buttonSair == null || promptSair == null)
        {
            Debug.LogError("PanelActivator: assign buttonSair and promptSair in the Inspector.");
            enabled = false;
            return;
        }

        promptSair.SetActive(false);

        buttonSair.onClick.AddListener(OnButtonSairClicked);
    }

    /*void Update()
    {
        if (!settings.activeSelf && !promptSair.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            promptSair.SetActive(true);
        }
    }*/

    void OnButtonSairClicked()
    {
        // Activate (show) the panel
        promptSair.SetActive(true);
    }
}
