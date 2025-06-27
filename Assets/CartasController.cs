using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CartasController : MonoBehaviour
{
    [SerializeField] Carta cartaPrefab;
    [SerializeField] Transform gridTransform;
    [SerializeField] Sprite[] sprites;

    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject videoDisplayCanvas;
    [SerializeField] private GameObject[] endGameObjects;

    [SerializeField] private string nextSceneName;

    public AudioSource flipUpSound;
    public AudioSource flipDownSound;
    public AudioSource flipDefaultSound;
    public AudioClip flipSound;

    private List<Sprite> spritePairs;

    Carta firstSelected;
    Carta secondSelected;

    int matchCounts;

    private bool isGameCompleted = false;

    private bool isBusy = false;

    private Coroutine showAllRoutine;
    private bool cardsRevealed = false;

    private bool isEndSequencePlaying = false;
    private bool hasSkippedEnd = false;

    private void Start()
    {
        /*if (PlayerPrefs.HasKey("SavedGameState"))
        {
            LoadGameState();
        }
        else
        {*/
            PrepareSprites();
            CreateCartas();
            //SaveGameStateIfNeeded();
            /*if (!cardsRevealed)
            {*/
                showAllRoutine = StartCoroutine(ShowAllCardsRoutine());
            //}
        //}
    }

    private void Update()
    {
        if (!cardsRevealed && Input.GetMouseButtonDown(0))
        {
            if (ClickedOnSkippableArea())
            {
                SkipCardShowRoutine();
            }
        }

        // NEW: Endgame click-to-skip
        if (isEndSequencePlaying && !hasSkippedEnd && Input.GetMouseButtonDown(0))
        {
            if (ClickedOnSkippableArea())
            {
                hasSkippedEnd = true;
            }
        }
    }

    void CreateCartasFromState(List<int> spriteIndices)
    {
        for (int i = 0; i < spriteIndices.Count; i++)
        {
            Carta carta = Instantiate(cartaPrefab, gridTransform);
            int spriteIndex = spriteIndices[i];
            if (spriteIndex >= 0 && spriteIndex < sprites.Length)
            {
                carta.SetIconSprite(sprites[spriteIndex]);
            }
            carta.controller = this;
        }
    }

    /*void LoadGameState()
    {
        foreach (Transform child in gridTransform)
        {
            child.GetComponent<Carta>().ResetState();
        }
        //firstSelected = null;
        //secondSelected = null;
        if (!PlayerPrefs.HasKey("SavedGameState"))
            return;

        string json = PlayerPrefs.GetString("SavedGameState");
        GameStateData state = JsonUtility.FromJson<GameStateData>(json);
        foreach (Transform child in gridTransform)
        {
            Destroy(child.gameObject);
        }
        CreateCartasFromState(state.cardSpriteIndices);

        bool isInitialEmptyState =
        state.matchedCardIndices.Count == 0 &&
        state.revealedCardIndices.Count == 0 &&
        state.firstSelectedIndex == -1 &&
        !state.cardsRevealed;


        if (isInitialEmptyState)
        {
            cardsRevealed = false; // Mark that they're about to be revealed and hidden again
            showAllRoutine = StartCoroutine(ShowAllCardsRoutine());
        }
        else
        {
            cardsRevealed = true; // Cards are already dealt with — no need to show/hide again
        }

        if (state == null || state.cardSpriteIndices.Count == 0) return;

        Debug.Log("Restoring saved game state.");

        for (int i = 0; i < state.cardSpriteIndices.Count; i++)
        {
            Carta carta = gridTransform.GetChild(i).GetComponent<Carta>();
            int spriteIndex = state.cardSpriteIndices[i];
            if (spriteIndex >= 0 && spriteIndex < sprites.Length)
            {
                carta.SetIconSprite(sprites[spriteIndex]);
            }
        }

        // Restore match count
        matchCounts = state.matchedCardIndices.Count / 2;

        // Restore matched cards
        foreach (int i in state.matchedCardIndices)
        {
            if (i >= 0 && i < gridTransform.childCount)
            {
                Carta carta = gridTransform.GetChild(i).GetComponent<Carta>();
                carta.GreyOut();
            }
        }

        // Restore flipped cards
        foreach (int i in state.revealedCardIndices)
        {
            if (i >= 0 && i < gridTransform.childCount)
            {
                Carta carta = gridTransform.GetChild(i).GetComponent<Carta>();
                carta.Show();
            }
        }

        cardsRevealed = state.cardsRevealed;

        // Prepare set of cards NOT to hide
        //HashSet<int> excludeIndices = new HashSet<int>(state.matchedCardIndices);

        // Reassign firstSelected properly
        if (state.firstSelectedIndex >= 0 && state.firstSelectedIndex < gridTransform.childCount)
        {
            //excludeIndices.Add(state.firstSelectedIndex);
            firstSelected = gridTransform.GetChild(state.firstSelectedIndex).GetComponent<Carta>();
            firstSelected.Show(); // Ensure it's visible
            firstSelected.isSelected = true;
        }
        else
        {
            firstSelected = null;
        }

        secondSelected = null;

        for (int i = 0; i < gridTransform.childCount; i++)
        {
            Carta carta = gridTransform.GetChild(i).GetComponent<Carta>();

            if (!state.revealedCardIndices.Contains(i))
            {
                carta.Show();
                carta.isSelected = true;
            }
            else
            {
                carta.Hide();
                carta.isSelected = false;
            }

            if (state.matchedCardIndices.Contains(i))
            {
                carta.GreyOut();
            }
        }
        isBusy = false;

        //showAllRoutine = StartCoroutine(ShowAllCardsRoutine(excludeIndices));
        /*int index = 0;
        foreach (Transform child in gridTransform)
        {
            Carta carta = child.GetComponent<Carta>();

            if (state.revealedCardIndices.Contains(index))
            {
                carta.Show();
            }

            if (state.matchedCardIndices.Contains(index))
            {
                carta.GreyOut();
            }

            index++;
        }

        matchCounts = state.matchCount;
        Debug.Log("Game state loaded.");
    }*/

    private bool ClickedOnSkippableArea()
    {
        // Raycast into the UI to check if we hit a specific UI element
        var pointerData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<UnityEngine.EventSystems.RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            // If we hit a UI object tagged "NoSkip", don't allow skipping
            if (result.gameObject.CompareTag("NoSkip"))
            {
                return false;
            }
        }

        // No "NoSkip" UI hit — okay to skip
        return true;
    }

    public void SkipCardShowRoutine()
    {
        if (showAllRoutine != null)
        {
            StopCoroutine(showAllRoutine);
            showAllRoutine = null;
        }

        foreach (Transform child in gridTransform)
        {
            var carta = child.GetComponent<Carta>();
            carta.Hide();
        }

        flipDownSound.Stop();
        flipDownSound.Play();

        cardsRevealed = true;
        //SaveGameStateIfNeeded();
    }

    IEnumerator ShowAllCardsRoutine(HashSet<int> excludeIndices = null)
    {
        foreach (Transform child in gridTransform)
        {
            var carta = child.GetComponent<Carta>();
            carta.Show();
        }
        flipUpSound.Stop();
        flipUpSound.Play();

        yield return new WaitForSeconds(4f);

        int index = 0;
        foreach (Transform child in gridTransform)
        {
            var carta = child.GetComponent<Carta>();
            if (excludeIndices == null || !excludeIndices.Contains(index))
            {
                carta.Hide();
            }

            index++;
        }
        flipDownSound.Stop();
        flipDownSound.Play();

        cardsRevealed = true;

        //SaveGameStateIfNeeded();
    }

    private void PrepareSprites()
    {
        spritePairs = new List<Sprite>();
        for(int i = 0; i < sprites.Length; i++)
        {
            spritePairs.Add(sprites[i]);
            spritePairs.Add(sprites[i]);
        }

        ShuffleSprites(spritePairs);
    }

    void CreateCartas()
    {
        for(int i = 0; i < spritePairs.Count; i++)
        {
            Carta carta = Instantiate(cartaPrefab, gridTransform);
            carta.SetIconSprite(spritePairs[i]);
            carta.controller = this;
        }
    }

    public void SetSelected(Carta carta)
    {
        Debug.Log($"Clicked card: {carta.name}, isSelected={carta.isSelected}"/*, isFlipping={carta.isFlipping}"*/);

        if (carta == null || isBusy || isGameCompleted /*|| carta.isFlipping*/)
            return; // Prevent interaction during comparison

        // Handle re-click to unselect
        if (carta == firstSelected && secondSelected == null)
        {
            Debug.Log("Re-clicking the first selected card to hide it.");
            carta.Hide();
            flipDefaultSound.PlayOneShot(flipSound);
            firstSelected = null;
            return;
        }
        //SaveGameStateIfNeeded();

        // Ignore clicks on already selected cards (not counting re-click above)
        if (carta.isSelected && carta != firstSelected)
            return;

        carta.Show();
        flipDefaultSound.PlayOneShot(flipSound);

        if (firstSelected == null)
        {
            firstSelected = carta;
            //SaveGameStateIfNeeded();
        }
        else if (secondSelected == null)
        {
            secondSelected = carta;
            StartCoroutine(CheckMatching(firstSelected, secondSelected));
            //firstSelected = null;
            //secondSelected = null;
        }
    }

    IEnumerator CheckMatching(Carta a, Carta b)
    {
        isBusy = true;

        yield return new WaitForSeconds(0.3f);
        if(a.iconSprite == b.iconSprite)
        {
            // Matched
            matchCounts++;
            a.GreyOut();
            b.GreyOut();
            if (matchCounts>= spritePairs.Count / 2)
            {
                isGameCompleted = true;
                PlayerPrefs.DeleteKey("SavedGameState");
                PrimeTween.Sequence.Create()
                    .Chain(PrimeTween.Tween.Scale(gridTransform, Vector3.one * 1.2f, 0.2f, ease: PrimeTween.Ease.OutBack))
                    .Chain(PrimeTween.Tween.Scale(gridTransform, Vector3.one, 0.1f));

                StartCoroutine(PlayEndSequence());
            }
        }
        else
        {
            // flip them back
            a.Hide();
            b.Hide();
            flipDownSound.Play();
        }

        firstSelected = null;
        secondSelected = null;
        isBusy = false;
    }

    IEnumerator PlayEndSequence()
    {
        isEndSequencePlaying = true;

        // Activate video
        videoDisplayCanvas.SetActive(true);
        videoPlayer.Play();

        // Pick a random GameObject
        int index = Random.Range(0, endGameObjects.Length);
        endGameObjects[index].SetActive(true);

        float waitTime = 4f;

        //yield return new WaitForSeconds(4f); // Or wait for videoPlayer.length if it's short

        float timer = 0f;
        while (timer < waitTime && !hasSkippedEnd)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    void ShuffleSprites(List<Sprite> spriteList)
    {
        for (int i = spriteList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            //Swap elements at i and randomIndex
            Sprite temp = spriteList[i];
            spriteList[i] = spriteList[randomIndex];
            spriteList[randomIndex] = temp;
        }
    }
    public void SaveGameStateIfNeeded()
    {
        if (isGameCompleted) return;

        GameStateData state = new GameStateData();

        int index = 0;
        state.cardSpriteIndices = new List<int>();

        foreach (Transform child in gridTransform)
        {
            Carta carta = child.GetComponent<Carta>();

            int spriteIndex = System.Array.IndexOf(sprites, carta.iconSprite);
            state.cardSpriteIndices.Add(spriteIndex);
            if (carta.isSelected)
                state.revealedCardIndices.Add(index);

            if (!carta.GetComponent<Button>().interactable)
                state.matchedCardIndices.Add(index);

            if (carta == firstSelected)
                state.firstSelectedIndex = index;

            index++;
        }

        state.matchCount = matchCounts;
        state.cardsRevealed = cardsRevealed;

        string json = JsonUtility.ToJson(state);
        PlayerPrefs.SetString("SavedGameState", json);
        PlayerPrefs.Save();

        Debug.Log("Saved game state before leaving scene.");
    }

    /*public void ResetSavedState()
    {
        PlayerPrefs.DeleteKey("SavedGameState");
        Debug.Log("Reset save game state");
    }*/

    public bool IsGameCompleted()
    {
        return isGameCompleted;
    }
}
