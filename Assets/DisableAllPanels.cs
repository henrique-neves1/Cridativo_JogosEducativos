using UnityEngine;

public class DisableAllPanels : MonoBehaviour
{
    // Array of panels to manage
    public GameObject[] panels;

    // Call this to disable all panels
    public void DisableAllPanels1()
    {
        foreach (GameObject panel in panels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }
}
