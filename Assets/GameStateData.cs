using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameStateData
{
    public List<int> revealedCardIndices = new List<int>(); // index in grid
    public List<int> matchedCardIndices = new List<int>();
    public List<int> cardSpriteIndices = new List<int>();
    public int matchCount = 0;
    public bool cardsRevealed = false;
    public int firstSelectedIndex = -1;
}