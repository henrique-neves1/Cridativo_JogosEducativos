using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MazeEndController : MonoBehaviour
{
    public GameObject rawImageObject;
    public VideoPlayer videoPlayer;
    public GameObject[] randomThingsToShow;

    public float timeBeforeSceneChange = 5f;
    public string nextSceneName = "NextScene";

    public PlayerController playerController;

    private bool isGameCompleted = false;
    private bool endSequenceActive = false;
    private float sceneChangeTime;

    public GenerateMaze mazeGenerator;

    public void TriggerEndSequence()
    {
        isGameCompleted = true;
        Debug.Log("TriggerEndSequence called");

        if (mazeGenerator != null)
            mazeGenerator.DeleteSave();

        if (playerController == null)
            Debug.LogWarning("playerController is null!");
        if (rawImageObject == null)
            Debug.LogWarning("rawImageObject is null!");
        if (videoPlayer == null)
            Debug.LogWarning("videoPlayer is null!");
        if (randomThingsToShow.Length == 0)
            Debug.LogWarning("randomThingsToShow is empty!");

        // Disable player control
        if (playerController != null)
            playerController.enabled = false;

        // Activate RawImage & play video
        rawImageObject.SetActive(true);
        Debug.Log("RawImage activated: " + rawImageObject.activeInHierarchy);
        if (videoPlayer != null)
            videoPlayer.Play();

        // Enable random object
        if (randomThingsToShow.Length > 0)
        {
            int index = Random.Range(0, randomThingsToShow.Length);
            randomThingsToShow[index].SetActive(true);
        }

        // Set up for user click skip
        endSequenceActive = true;
        sceneChangeTime = Time.time + timeBeforeSceneChange;

        // Automatic fallback
        Invoke(nameof(LoadNextScene), timeBeforeSceneChange);
    }

    private void Update()
    {
        if (!endSequenceActive) return;

        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            // First check if the click was on a UI element tagged "NoSkip"
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var raycastResults = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            foreach (var result in raycastResults)
            {
                if (result.gameObject.CompareTag("NoSkip"))
                {
                    Debug.Log("Clicked on NoSkip UI element, ignoring skip.");
                    return;
                }
            }

            // Then check for 2D world objects with a collider
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.CompareTag("NoSkip"))
            {
                Debug.Log("Clicked on NoSkip 2D object, ignoring skip.");
                return;
            }

            Debug.Log("User clicked to skip end sequence.");
            CancelInvoke(nameof(LoadNextScene)); // Stop auto delay
            LoadNextScene();
        }
    }

    public bool IsGameCompleted()
    {
        return isGameCompleted;
    }

    private void LoadNextScene()
    {
        if (!endSequenceActive) return;
        endSequenceActive = false;

        Debug.Log("Loading scene: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}