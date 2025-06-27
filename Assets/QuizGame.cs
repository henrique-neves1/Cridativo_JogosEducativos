using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using PrimeTween;

public class QuizGame : MonoBehaviour
{
    public Button[] answerButtons;
    public int correctIndex;
    public GameObject[] correctResponses;
    public GameObject[] wrongResponses;
    public AudioClip correctSFX, wrongSFX;
    public AudioSource sfxPlayer;
    public float delayBeforeSceneLoad = 2f;
    public string nextSceneName;
    public bool allowSkip = false;
    bool sceneLoaded = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; // Local copy for lambda
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }
    }

    void Update()
    {
        if (allowSkip && !sceneLoaded && Input.GetMouseButtonDown(0))
        {
            // Check if clicked UI with NoSkip tag
            if (!ClickedNoSkipUI())
            {
                LoadSceneNow();
            }
        }
    }

    void OnAnswerSelected(int index)
    {
        DisableAllButtons();
        if (index == correctIndex)
        {
            StartCoroutine(HandleCorrect(answerButtons[index]));
        }
        else
        {
            StartCoroutine(HandleWrong(answerButtons[index]));
        }
    }

    void DisableAllButtons()
    {
        foreach (var btn in answerButtons)
            btn.interactable = false;
    }

    IEnumerator HandleWrong(Button button)
    {
        // Show X
        Transform xMark = button.transform.Find("X");
        if (xMark == null)
        {
            yield break;
        }

        GameObject x = xMark.gameObject;

        // Play SFX
        sfxPlayer.PlayOneShot(wrongSFX);

        // Activate random wrong response
        ActivateRandomResponse(wrongResponses);

        for (int i = 0; i < 2; i++)
        {
            x.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            x.SetActive(false);
            yield return new WaitForSeconds(0.25f);
        }

        x.SetActive(true); // Final blink stays visible slightly longer
        yield return new WaitForSeconds(0.5f);
        x.SetActive(false); // Hide it again

        foreach (var response in wrongResponses)
        {
            response.SetActive(false);
        }

        // Re-enable all buttons
        foreach (var btn in answerButtons)
            btn.interactable = true;
    }

    IEnumerator HandleCorrect(Button button)
    {
        // Show and animate Checkmark
        Transform check = button.transform.Find("Checkmark");

        sfxPlayer.PlayOneShot(correctSFX);

        ActivateRandomResponse(correctResponses);

        if (check)
        {
            check.gameObject.SetActive(true);
            check.localScale = Vector3.zero;
            Tween.Scale(check, Vector3.one, 0.7f, Ease.OutBack);
            /*float time = 0f;
            while (time < 0.5f)
            {
                time += Time.deltaTime;
                float scale = Mathf.SmoothStep(0f, 1f, time / 0.5f); // Ease out
                check.localScale = new Vector3(scale, scale, scale);
                yield return null;
            }*/
        }

        allowSkip = true;

        float elapsed = 0f;
        while (elapsed < delayBeforeSceneLoad && !sceneLoaded)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        LoadSceneNow();
    }

    bool ClickedNoSkipUI()
    {
        // Check if clicked on a UI element with "NoSkip" tag
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            var raycastResults = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
            UnityEngine.EventSystems.PointerEventData pointerData = new(UnityEngine.EventSystems.EventSystem.current)
            {
                position = Input.mousePosition
            };
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerData, raycastResults);

            foreach (var result in raycastResults)
            {
                if (result.gameObject.CompareTag("NoSkip"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    void LoadSceneNow()
    {
        sceneLoaded = true;
        allowSkip = false;
        SceneManager.LoadScene(nextSceneName);
    }

    void ActivateRandomResponse(GameObject[] responses)
    {
        int i = Random.Range(0, responses.Length);
        for (int j = 0; j < responses.Length; j++)
        {
            responses[j].SetActive(j == i);
        }
    }
}
