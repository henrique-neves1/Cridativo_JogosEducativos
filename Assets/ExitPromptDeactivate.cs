using UnityEngine;
using UnityEngine.UI;

public class ExitPromptDeactivate : MonoBehaviour
{
    public Button buttonNao;
    public GameObject promptSair;

    private void Start()
    {
        if (buttonNao == null || promptSair == null)
        {
            Debug.LogError("PanelActivator: assign buttonSair and promptSair in the Inspector.");
            enabled = true;
            return;
        }

        promptSair.SetActive(true);

        buttonNao.onClick.AddListener(OnButtonNaoClicked);
    }

    void Update()
    {
        if (promptSair.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            promptSair.SetActive(false);
        }
    }

    void OnButtonNaoClicked()
    {
        // Activate (show) the panel
        promptSair.SetActive(false);
    }
}
