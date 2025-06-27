using UnityEngine;
using UnityEngine.SceneManagement;

public class Congratulations : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string nextSceneName = "NextScene";
    [SerializeField] private float delayBeforeLoad = 3f;

    [Header("Random GameObjects")]
    [SerializeField] private GameObject[] randomObjects;

    private bool sceneLoaded = false;

    void Start()
    {
        ActivateRandomObject();
        Invoke("LoadNextScene", delayBeforeLoad);
    }

    void Update()
    {
        if (!sceneLoaded && Input.GetMouseButtonDown(0))
        {
            CancelInvoke("LoadNextScene");
            LoadNextScene();
        }
    }

    void ActivateRandomObject()
    {
        if (randomObjects != null && randomObjects.Length > 0)
        {
            int index = Random.Range(0, randomObjects.Length);
            randomObjects[index].SetActive(true);
        }
    }

    void LoadNextScene()
    {
        if (sceneLoaded) return;
        sceneLoaded = true;

        SceneManager.LoadScene(nextSceneName);
    }
}
