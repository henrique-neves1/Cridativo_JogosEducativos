using UnityEngine;
using System.Collections.Generic;

public class RoomControllerUI : MonoBehaviour
{
    public GameObject topWall;
    public GameObject bottomWall;
    public GameObject leftWall;
    public GameObject rightWall;

    public int gridX, gridY;

    public void RemoveWall(string wall)
    {
        switch (wall)
        {
            case "Top": if (topWall) topWall.SetActive(false); break;
            case "Bottom": if (bottomWall) bottomWall.SetActive(false); break;
            case "Left": if (leftWall) leftWall.SetActive(false); break;
            case "Right": if (rightWall) rightWall.SetActive(false); break;
        }
    }

    public void RemoveRandomOuterWall(int totalRows, int totalCols)
    {
        List<string> outerWalls = new List<string>();

        if (gridY == 0) outerWalls.Add("Top");
        if (gridY == totalRows - 1) outerWalls.Add("Bottom");
        if (gridX == 0) outerWalls.Add("Left");
        if (gridX == totalCols - 1) outerWalls.Add("Right");

        if (outerWalls.Count > 0)
        {
            string wall = outerWalls[Random.Range(0, outerWalls.Count)];
            RemoveWall(wall);
        }
    }
}
