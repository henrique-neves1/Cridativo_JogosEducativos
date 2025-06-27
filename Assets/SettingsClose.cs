using UnityEngine;
using UnityEngine.UI;

public class SettingsClose : MonoBehaviour
{
    public Button buttonVoltar;
    public GameObject definicoes;

    private void Start()
    {
        if (buttonVoltar == null || definicoes == null)
        {
            Debug.LogError("PanelActivator: assign buttonDefinicoes and definicoes in the Inspector.");
            enabled = true;
            return;
        }

        definicoes.SetActive(true);

        buttonVoltar.onClick.AddListener(OnButtonVoltarClicked);
    }

    void OnButtonVoltarClicked()
    {
        // Activate (show) the panel
        definicoes.SetActive(false);
    }
}
