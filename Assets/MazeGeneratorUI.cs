using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MazeGeneratorUI : MonoBehaviour
{
    public GameObject roomPrefab;
    public RectTransform gridContainer;
    public int rows = 5;
    public int columns = 5;

    private RoomControllerUI[,] grid;
    private List<RoomControllerUI> allRooms = new List<RoomControllerUI>();

    void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
    {
        grid = new RoomControllerUI[rows, columns];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                GameObject roomGO = Instantiate(roomPrefab, gridContainer);
                roomGO.name = $"Room_{y}_{x}";

                RoomControllerUI rc = roomGO.GetComponent<RoomControllerUI>();
                rc.gridX = x;
                rc.gridY = y;
                grid[y, x] = rc;
                allRooms.Add(rc);
            }
        }

        RemoveWallsRandomly();
        SetEntranceAndExit();
    }

    void RemoveWallsRandomly()
    {
        foreach (RoomControllerUI room in allRooms)
        {
            List<string> validWalls = new List<string>();

            int x = room.gridX;
            int y = room.gridY;

            // Check which internal walls are safe to remove (not on maze edge)
            if (y > 0) validWalls.Add("Top");
            if (y < rows - 1) validWalls.Add("Bottom");
            if (x > 0) validWalls.Add("Left");
            if (x < columns - 1) validWalls.Add("Right");

            int wallsToRemove = Random.Range(1, Mathf.Min(validWalls.Count, 4)); // max 3 if possible

            for (int i = 0; i < wallsToRemove; i++)
            {
                if (validWalls.Count == 0) break;

                string wall = validWalls[Random.Range(0, validWalls.Count)];
                validWalls.Remove(wall);
                room.RemoveWall(wall);

                // Remove the corresponding wall from the adjacent room
                switch (wall)
                {
                    case "Top":
                        grid[y - 1, x]?.RemoveWall("Bottom");
                        break;
                    case "Bottom":
                        grid[y + 1, x]?.RemoveWall("Top");
                        break;
                    case "Left":
                        grid[y, x - 1]?.RemoveWall("Right");
                        break;
                    case "Right":
                        grid[y, x + 1]?.RemoveWall("Left");
                        break;
                }
            }
        }
    }

    void SetEntranceAndExit()
    {
        RoomControllerUI entrance = allRooms[Random.Range(0, allRooms.Count)];
        RoomControllerUI exit;

        do
        {
            exit = allRooms[Random.Range(0, allRooms.Count)];
        } while (exit == entrance);

        entrance.RemoveRandomOuterWall(rows, columns);
        exit.RemoveRandomOuterWall(rows, columns);
    }
}