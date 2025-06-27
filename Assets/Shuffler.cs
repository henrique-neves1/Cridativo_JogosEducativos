using UnityEngine;

public class Shuffler : MonoBehaviour
{
    public void Shuffle()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            transform.GetChild(i).SetSiblingIndex(Random.Range(0, transform.childCount));
        }
    }
}