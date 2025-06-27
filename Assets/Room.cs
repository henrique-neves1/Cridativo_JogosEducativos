using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum Directions
    {
        TOP,
        RIGHT,
        BOTTOM,
        LEFT,
        NONE,
    }

    [SerializeField]
    public GameObject topWall;
    [SerializeField]
    public GameObject rightWall;
    [SerializeField]
    public GameObject bottomWall;
    [SerializeField]
    public GameObject leftWall;

    Dictionary<Directions, GameObject> walls =
        new Dictionary<Directions, GameObject>();

    public Vector2Int Index
    {
        get;
        set;
    }

    public bool visited { get; set; } = false;

    Dictionary<Directions, bool> dirflags =
        new Dictionary<Directions, bool>();

    private void Awake()
    {
        if (topWall == null) topWall = transform.Find("Top")?.gameObject;
        if (rightWall == null) rightWall = transform.Find("Right")?.gameObject;
        if (bottomWall == null) bottomWall = transform.Find("Bottom")?.gameObject;
        if (leftWall == null) leftWall = transform.Find("Left")?.gameObject;

        walls[Directions.TOP] = topWall;
        walls[Directions.RIGHT] = rightWall;
        walls[Directions.BOTTOM] = bottomWall;
        walls[Directions.LEFT] = leftWall;
    }

    public bool HasWall(Room.Directions dir)
    {
        switch (dir)
        {
            case Directions.TOP: return topWall.activeSelf;
            case Directions.RIGHT: return rightWall.activeSelf;
            case Directions.BOTTOM: return bottomWall.activeSelf;
            case Directions.LEFT: return leftWall.activeSelf;
            default: return false;
        }
    }

    private void SetActive(Directions dir, bool flag)
    {
        walls[dir].SetActive(flag);
    }

    public void SetDirFlag(Directions dir, bool /*flag*/ exists)
    {
        Debug.Log($"Setting {dir} wall on Room {Index} to {(exists ? "ACTIVE" : "INACTIVE")}");

        /*dirflags[dir] = flag;
        SetActive(dir, flag);*/
        switch (dir)
        {
            case Directions.TOP:
                topWall.SetActive(exists);
                break;
            case Directions.RIGHT:
                rightWall.SetActive(exists);
                break;
            case Directions.BOTTOM:
                bottomWall.SetActive(exists);
                break;
            case Directions.LEFT:
                leftWall.SetActive(exists);
                break;
        }
    }
}
