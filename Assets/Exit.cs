using UnityEngine;
using UnityEngine.UI;

public class Exit : MonoBehaviour
{
    public GameObject exitPanel;

    public void OnQuitButtonPressed()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
