using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    public GameObject roomPrefab;
    public int rows = 5;
    public int columns = 5;
    public float roomSpacing = 1.1f;

    private RoomController[,] grid;

    private List<RoomController> allRooms = new List<RoomController>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
    {
        grid = new RoomController[rows, columns];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector2 pos = new Vector2(x * roomSpacing, -y * roomSpacing);
                GameObject roomObj = Instantiate(roomPrefab, pos, Quaternion.identity);
                roomObj.name = $"Room_{y}_{x}";

                RoomController rc = roomObj.GetComponent<RoomController>();
                grid[y, x] = rc;
                allRooms.Add(rc);

                rc.gridX = x;
                rc.gridY = y;
            }
        }

        RemoveWallsRandomly();

        // Random entrance and exit
        SetEntranceAndExit();
    }

    void RemoveWallsRandomly()
    {
        foreach (RoomController room in allRooms)
        {
            List<string> wallNames = new List<string> { "Top", "Bottom", "Left", "Right" };
            int wallsToRemove = Random.Range(1, 4);

            for (int i = 0; i < wallsToRemove; i++)
            {
                if (wallNames.Count == 0) break;

                string wall = wallNames[Random.Range(0, wallNames.Count)];
                wallNames.Remove(wall);
                room.RemoveWall(wall);

                // Synchronize adjacent room's wall
                int x = room.gridX;
                int y = room.gridY;

                switch (wall)
                {
                    case "Top":
                        if (y > 0) grid[y - 1, x]?.RemoveWall("Bottom");
                        break;
                    case "Bottom":
                        if (y < rows - 1) grid[y + 1, x]?.RemoveWall("Top");
                        break;
                    case "Left":
                        if (x > 0) grid[y, x - 1]?.RemoveWall("Right");
                        break;
                    case "Right":
                        if (x < columns - 1) grid[y, x + 1]?.RemoveWall("Left");
                        break;
                }
            }
        }
    }

    void SetEntranceAndExit()
    {
        RoomController entrance = allRooms[Random.Range(0, allRooms.Count)];
        RoomController exit;

        do
        {
            exit = allRooms[Random.Range(0, allRooms.Count)];
        } while (exit == entrance);

        entrance.RemoveRandomOuterWall(rows, columns);
        exit.RemoveRandomOuterWall(rows, columns);
    }
}
