using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Feedback")]
    public GameObject[] correctFeedbacks;
    public GameObject[] wrongFeedbacks;
    public float feedbackDuration = 2f;

    [Header("Completion")]
    public RawImage videoOverlay;
    public VideoPlayer videoPlayer;
    public float autoLoadDelay = 5f;
    public string sceneToLoad = "MenuPrincipal";

    public bool gameComplete = false;

    void Start()
    {
        SaveData data = LoadSaveData();

        bool hasSave = data != null && (data.revealedSlots.Count > 0 || data.destroyedTexts.Count > 0);

        if (hasSave)
        {
            RestoreState(data);
        }
        else
        {
            // Shuffle only if there's no saved state
            foreach (Shuffler shuffler in FindObjectsOfType<Shuffler>())
            {
                shuffler.Shuffle();

                // Immediately save the shuffled layout state
                SaveProgress("");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameComplete && Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverNoSkipTag())
            {
                LoadNextScene();
            }
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public List<string> revealedSlots = new List<string>();
        public List<string> destroyedTexts = new List<string>();

        public List<DraggableOrderData> draggableOrders = new List<DraggableOrderData>();
        public List<SlotOrderData> slotOrders = new List<SlotOrderData>();
    }

    [System.Serializable]
    public class DraggableOrderData
    {
        public string numberValue;
        public int siblingIndex;
    }

    [System.Serializable]
    public class SlotOrderData
    {
        public string correctTextNumber;
        public int siblingIndex;
    }

    private Dictionary<Transform, List<T>> GroupByParent<T>(IEnumerable<T> objects) where T : MonoBehaviour
    {
        Dictionary<Transform, List<T>> grouped = new Dictionary<Transform, List<T>>();
        foreach (var obj in objects)
        {
            Transform parent = obj.transform.parent;
            if (!grouped.ContainsKey(parent))
                grouped[parent] = new List<T>();

            grouped[parent].Add(obj);
        }
        return grouped;
    }

    private const string SaveKey = "NumberGameSave";

    public void SaveProgress(string revealedSlotValue)
    {
        SaveData data = LoadSaveData();

        if (!data.revealedSlots.Contains(revealedSlotValue))
            data.revealedSlots.Add(revealedSlotValue);

        if (!data.destroyedTexts.Contains(revealedSlotValue))
            data.destroyedTexts.Add(revealedSlotValue);

        // Save sibling indexes of remaining draggable texts
        data.draggableOrders.Clear();
        foreach (DraggableText dt in FindObjectsOfType<DraggableText>())
        {
            if (dt != null && dt.gameObject.activeInHierarchy)
            {
                data.draggableOrders.Add(new DraggableOrderData
                {
                    numberValue = dt.numberValue,
                    siblingIndex = dt.transform.GetSiblingIndex()
                });
            }
        }

        // Save number slot sibling indexes
        data.slotOrders.Clear();
        foreach (NumberSlot slot in FindObjectsOfType<NumberSlot>())
        {
            data.slotOrders.Add(new SlotOrderData
            {
                correctTextNumber = slot.correctTextNumber,
                siblingIndex = slot.transform.GetSiblingIndex()
            });
        }

        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    private void SaveInitialLayoutState()
    {
        SaveData data = new SaveData();

        // Save draggable texts sibling indexes
        foreach (DraggableText dt in FindObjectsOfType<DraggableText>())
        {
            if (dt != null && dt.gameObject.activeInHierarchy)
            {
                data.draggableOrders.Add(new DraggableOrderData
                {
                    numberValue = dt.numberValue,
                    siblingIndex = dt.transform.GetSiblingIndex()
                });
            }
        }

        // Save number slots sibling indexes
        foreach (NumberSlot slot in FindObjectsOfType<NumberSlot>())
        {
            data.slotOrders.Add(new SlotOrderData
            {
                correctTextNumber = slot.correctTextNumber,
                siblingIndex = slot.transform.GetSiblingIndex()
            });
        }

        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public SaveData LoadSaveData()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            return JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(SaveKey));
        }
        return new SaveData();
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
    }

    private void RestoreState(SaveData data)
    {
        // Hide matched draggable texts
        foreach (DraggableText dt in FindObjectsOfType<DraggableText>())
        {
            if (data.destroyedTexts.Contains(dt.numberValue))
            {
                Destroy(dt.gameObject);
            }
        }

        // Show correct text reveals
        foreach (NumberSlot slot in FindObjectsOfType<NumberSlot>())
        {
            if (data.revealedSlots.Contains(slot.correctTextNumber))
            {
                slot.textReveal.SetActive(true);
            }
        }

        // === DraggableTexts restore ===
        Dictionary<string, DraggableText> draggableLookup = new Dictionary<string, DraggableText>();
        foreach (DraggableText dt in FindObjectsOfType<DraggableText>())
        {
            if (!draggableLookup.ContainsKey(dt.numberValue))
                draggableLookup[dt.numberValue] = dt;
        }

        foreach (var group in GroupByParent(draggableLookup.Values))
        {
            var parent = group.Key;
            var items = group.Value;

            // Match saved order data
            List<(DraggableText dt, int index)> ordered = new List<(DraggableText, int)>();
            foreach (var item in items)
            {
                var order = data.draggableOrders.Find(o => o.numberValue == item.numberValue);
                if (order != null)
                    ordered.Add((item, order.siblingIndex));
            }

            ordered.Sort((a, b) => a.index.CompareTo(b.index));

            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].dt.transform.SetSiblingIndex(i);
            }
        }

        // === NumberSlots restore ===
        Dictionary<string, NumberSlot> slotLookup = new Dictionary<string, NumberSlot>();
        foreach (NumberSlot slot in FindObjectsOfType<NumberSlot>())
        {
            if (!slotLookup.ContainsKey(slot.correctTextNumber))
                slotLookup[slot.correctTextNumber] = slot;
        }

        foreach (var group in GroupByParent(slotLookup.Values))
        {
            var parent = group.Key;
            var items = group.Value;

            List<(NumberSlot slot, int index)> ordered = new List<(NumberSlot, int)>();
            foreach (var item in items)
            {
                var order = data.slotOrders.Find(o => o.correctTextNumber == item.correctTextNumber);
                if (order != null)
                    ordered.Add((item, order.siblingIndex));
            }

            ordered.Sort((a, b) => a.index.CompareTo(b.index));

            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].slot.transform.SetSiblingIndex(i);
            }
        }
    }

    public void ShowRandomFeedback(bool correct)
    {
        foreach(GameObject correctFeedback in correctFeedbacks)
        {
            correctFeedback.SetActive(false);
        }

        foreach (GameObject wrongFeedback in wrongFeedbacks)
        {
            wrongFeedback.SetActive(false);
        }

        GameObject[] feedbackArray = correct ? correctFeedbacks : wrongFeedbacks;
        if (feedbackArray.Length == 0) return;

        int index = Random.Range(0, feedbackArray.Length);
        GameObject feedback = feedbackArray[index];
        StartCoroutine(ShowFeedbackTemp(feedback));
    }

    private IEnumerator ShowFeedbackTemp(GameObject obj)
    {
        obj.SetActive(true);
        AudioSource audio = obj.GetComponent<AudioSource>();
        if (audio) audio.Play();

        yield return new WaitForSeconds(feedbackDuration);

        obj.SetActive(false);
    }

    public void CheckForCompletion()
    {
        if (gameComplete) return;

        DraggableText[] remaining = GameObject.FindObjectsOfType<DraggableText>();
        bool allTextGone = remaining.Length == 0 || AllInactive(remaining);

        bool allRevealsActive = true;
        foreach (NumberSlot slot in FindObjectsOfType<NumberSlot>())
        {
            if (!slot.textReveal.activeSelf)
            {
                allRevealsActive = false;
                break;
            }
        }

        if (allTextGone && allRevealsActive)
        {
            Debug.Log("Game completed!");
            StartCoroutine(HandleCompletion());
        }
    }

    private bool AllInactive(DraggableText[] texts)
    {
        foreach (var t in texts)
        {
            if (t.gameObject.activeInHierarchy)
                return false;
        }
        return true;
    }

    private IEnumerator HandleCompletion()
    {
        DeleteSave(); // clear saved state
        gameComplete = true;
        videoOverlay.gameObject.SetActive(true);
        videoPlayer.Play();

        yield return new WaitForSeconds(autoLoadDelay);

        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    private bool IsPointerOverNoSkipTag()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            var raycastResults = new List<UnityEngine.EventSystems.RaycastResult>();
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(
                new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current)
                {
                    position = Input.mousePosition
                },
                raycastResults
            );

            foreach (var result in raycastResults)
            {
                if (result.gameObject.CompareTag("NoSkip"))
                    return true;
            }
        }

        return false;
    }

    void OnApplicationQuit()
    {
        if (!gameComplete)
        {
            SaveProgress("");
        }
    }
}
