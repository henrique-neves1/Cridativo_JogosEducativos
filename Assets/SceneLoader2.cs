using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader2 : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        //TrySaveGameState();
        SceneManager.LoadScene(sceneName);
    }

    /*private void TrySaveGameState()
    {
        // Only save if we're currently in the game scene
        CartasController controller = FindObjectOfType<CartasController>();
        if (controller != null)
        {
            controller.SaveGameStateIfNeeded();
        }
    }*/
}
