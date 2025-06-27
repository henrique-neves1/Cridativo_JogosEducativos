using UnityEngine;
using UnityEngine.UI;

public class SettingsOpen : MonoBehaviour
{
    public Button buttonDefinicoes;
    public GameObject definicoes;

    private void Start()
    {
        if (buttonDefinicoes == null || definicoes == null)
        {
            Debug.LogError("PanelActivator: assign buttonDefinicoes and definicoes in the Inspector.");
            enabled = false;
            return;
        }

        definicoes.SetActive(false);

        buttonDefinicoes.onClick.AddListener(OnButtonDefinicoesClicked);
    }

    void OnButtonDefinicoesClicked()
    {
        // Activate (show) the panel
        definicoes.SetActive(true);
    }
}
