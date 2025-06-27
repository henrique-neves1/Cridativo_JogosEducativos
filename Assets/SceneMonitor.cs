using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMonitor : MonoBehaviour
{
    private static SceneMonitor instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}