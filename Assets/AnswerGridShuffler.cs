using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AnswerGridShuffler : MonoBehaviour
{
    void Awake()
    {
        List<Transform> children = new();

        // Collect child transforms
        foreach (Transform child in transform)
        {
            children.Add(child);
        }

        // Shuffle them using Fisher–Yates
        for (int i = 0; i < children.Count; i++)
        {
            int rand = Random.Range(i, children.Count);
            (children[i], children[rand]) = (children[rand], children[i]);
        }

        // Apply the new order via sibling index
        for (int i = 0; i < children.Count; i++)
        {
            children[i].SetSiblingIndex(i);
        }
    }
}
