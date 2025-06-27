using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GenerateMazeMedium : MonoBehaviour
{
    [SerializeField]
    GameObject roomPrefab;

    // The grid
    Room[,] rooms = null;

    [SerializeField]
    int numX = 5;
    [SerializeField]
    int numY = 5;

    [SerializeField]
    Transform roomContainer;

    [SerializeField]
    GameObject playerPrefab;

    public GameObject[] blockingPanels;

    private GameObject playerInstance;

    // The room width and height
    float roomWidth;
    float roomHeight;

    // The stack for backtracking
    Stack<Room> stack = new Stack<Room>();

    bool generating = false;

    public MazeEndControllerMedium yourMazeEndControllerReference;

    private void GetRoomSize()
    {
        SpriteRenderer[] spriteRenderers =
            roomPrefab.GetComponentsInChildren<SpriteRenderer>();

        Vector3 minBounds = Vector3.positiveInfinity;
        Vector3 maxBounds = Vector3.negativeInfinity;

        foreach(SpriteRenderer ren in spriteRenderers)
        {
            minBounds = Vector3.Min(
                minBounds,
                ren.bounds.min);

            maxBounds = Vector3.Max(
                maxBounds,
                ren.bounds.max);
        }

        roomWidth = maxBounds.x - minBounds.x;
        roomHeight = maxBounds.y - minBounds.y;
    }

    /*private void SetCamera()
    {
        Vector3 centerPos = new Vector3(
        (numX - 1) * roomWidth / 2f,
        (numY - 1) * roomHeight / 2f,
        -10f); // -10 is a common Z offset for orthographic cam

        Camera.main.transform.position = centerPos;

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float widthSize = numX * roomWidth / 2f;
        float heightSize = numY * roomHeight / 2f;

        // Adjust to fit both width and height
        Camera.main.orthographicSize = Mathf.Max(heightSize, widthSize / screenRatio);
    }*/

    private void Start()
    {
        GetRoomSize();

        rooms = new Room[numX, numY];

        for(int i = 0; i < numX; ++i)
        {
            for(int j = 0; j < numY; ++j)
            {
                float offsetX = (numX - 1) * roomWidth / 2f;
                float offsetY = (numY - 1) * roomHeight / 2f;

                Vector2 anchoredPos = new Vector2(i * roomWidth - offsetX, j * roomHeight - offsetY);
                GameObject room = Instantiate(roomPrefab, roomContainer);
                RectTransform rt = room.GetComponent<RectTransform>();
                rt.anchoredPosition = anchoredPos;

                room.name = "Room_" + i.ToString() + "_" + j.ToString();
                rooms[i, j] = room.GetComponent<Room>();
                rooms[i, j].Index = new Vector2Int(i, j);
            }
        }

        /*roomContainer.GetComponent<RectTransform>().anchoredPosition =
        new Vector2(-(numX - 1) * roomWidth / 2f, -(numY - 1) * roomHeight / 2f);*/

        if (!LoadMazeState())
        {
            CreateMaze();
            SaveMazeState(new Vector2Int(0, 0));
        }
    }

    private void RemoveRoomWall(
        int x,
        int y,
        Room.Directions dir)
    {
        if (dir != Room.Directions.NONE)
        {
            rooms[x, y].SetDirFlag(dir, false);
        }

        Room.Directions opp = Room.Directions.NONE;
        switch (dir)
        {
            case Room.Directions.TOP:
                if(y < numY - 1)
                {
                    opp = Room.Directions.BOTTOM;
                    ++y;
                }
                break;
            case Room.Directions.RIGHT:
                if (x < numX - 1)
                {
                    opp = Room.Directions.LEFT;
                    ++x;
                }
                break;
            case Room.Directions.BOTTOM:
                if (y > 0)
                {
                    opp = Room.Directions.TOP;
                    --y;
                }
                break;
            case Room.Directions.LEFT:
                if (x > 0)
                {
                    opp = Room.Directions.RIGHT;
                    --x;
                }
                break;
        }
        if(opp != Room.Directions.NONE)
        {
            rooms[x, y].SetDirFlag(opp, false);
        }
    }

    public List<Tuple<Room.Directions, Room>> GetNeighboursNotVisited(
        int cx, int cy)
    {
        List<Tuple<Room.Directions, Room>> neighbours =
            new List<Tuple<Room.Directions, Room>>();

        foreach(Room.Directions dir in Enum.GetValues(
            typeof(Room.Directions)))
        {
            int x = cx;
            int y = cy;

            switch(dir)
            {
                case Room.Directions.TOP:
                    if(y < numY - 1)
                    {
                        ++y;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(
                                Room.Directions.TOP,
                                rooms[x, y]));
                        }
                    }
                    break;
                case Room.Directions.RIGHT:
                    if (x < numX - 1)
                    {
                        ++x;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(
                                Room.Directions.RIGHT,
                                rooms[x, y]));
                        }
                    }
                    break;
                case Room.Directions.BOTTOM:
                    if (y > 0)
                    {
                        --y;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(
                                Room.Directions.BOTTOM,
                                rooms[x, y]));
                        }
                    }
                    break;
                case Room.Directions.LEFT:
                    if (x > 0)
                    {
                        --x;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(
                                Room.Directions.LEFT,
                                rooms[x, y]));
                        }
                    }
                    break;
            }
        }
        return neighbours;
    }
    
    private bool GenerateStep()
    {
        if (stack.Count == 0) return true;

        Room r = stack.Peek();
        var neighbours = GetNeighboursNotVisited(r.Index.x, r.Index.y);

        if(neighbours.Count != 0)
        {
            var index = 0;
            if(neighbours.Count > 1)
            {
                index = UnityEngine.Random.Range(0, neighbours.Count);
            }

            var item = neighbours[index];
            Room neighbour = item.Item2;
            neighbour.visited = true;
            RemoveRoomWall(r.Index.x, r.Index.y, item.Item1);

            stack.Push(neighbour);
        }
        else
        {
            stack.Pop();
        }

        return false;
    }

    public void CreateMaze()
    {
        if (generating) return;

        Reset();

        // Open bottom wall of starting room (0, 0)
        RemoveRoomWall(0, 0, Room.Directions.BOTTOM);
        rooms[0, 0].bottomWall.GetComponent<Collider2D>().enabled = true;
        rooms[0, 0].bottomWall.GetComponent<Image>().enabled = false; // invisible for entry

        // Open right wall of bottom-right room (exit point)
        RemoveRoomWall(numX - 1, numY - 1, Room.Directions.RIGHT);

        // Maze generation
        stack.Push(rooms[0, 0]);
        rooms[0, 0].visited = true;
        generating = true;
        while (!GenerateStep()) { }
        generating = false;

        // Spawn player at (0, 0)
        if (playerInstance != null)
            Destroy(playerInstance);

        Vector3 spawnPos = rooms[0, 0].transform.position;
        playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity, roomContainer);

        PlayerControllerMedium pc = playerInstance.GetComponent<PlayerControllerMedium>();
        if (pc != null)
        {
            pc.mazeGenerator = this;
            pc.roomWidth = roomWidth;
            pc.roomHeight = roomHeight;
            pc.SetRoomGrid(rooms);
            pc.blockingPanels = blockingPanels;
            pc.SetPlayerIndex(Vector2Int.zero);
        }

        yourMazeEndControllerReference.playerController = pc;

        // === Invisible Exit Room Setup ===
        GameObject invisibleExitRoom = new GameObject("InvisibleExitRoom");
        invisibleExitRoom.transform.SetParent(roomContainer, false);

        RectTransform invisibleRT = invisibleExitRoom.AddComponent<RectTransform>();
        invisibleRT.sizeDelta = new Vector2(roomWidth, roomHeight);

        float offsetX = (numX - 1) * roomWidth / 2f;
        float offsetY = (numY - 1) * roomHeight / 2f;
        Vector2 invisibleRoomPos = new Vector2(numX * roomWidth - offsetX, (numY - 1) * roomHeight - offsetY);
        invisibleRT.anchoredPosition = invisibleRoomPos;

        // Nested Exit Trigger inside invisible room
        GameObject exitTriggerGO = new GameObject("FinalExitTrigger");
        exitTriggerGO.transform.SetParent(invisibleExitRoom.transform, false);

        RectTransform exitRT = exitTriggerGO.AddComponent<RectTransform>();
        exitRT.anchorMin = Vector2.zero;
        exitRT.anchorMax = Vector2.one;
        exitRT.offsetMin = Vector2.zero;
        exitRT.offsetMax = Vector2.zero;

        ExitTriggerMedium invisibleTrigger = exitTriggerGO.AddComponent<ExitTriggerMedium>();
        invisibleTrigger.mazeEndController = yourMazeEndControllerReference;

        BoxCollider2D invisibleCol = exitTriggerGO.AddComponent<BoxCollider2D>();
        invisibleCol.isTrigger = true;

        Image invisibleImg = exitTriggerGO.AddComponent<Image>();
        invisibleImg.color = new Color(0, 0, 0, 0); // fully transparent
    }

    /*IEnumerator Coroutine_Generate()
    {
        generating = true;
        bool flag = false;
        while (!flag)
        {
            flag = GenerateStep();
            yield return new WaitForSeconds(0.05f);
        }

        generating = false;
    }*/

    string savePath => Path.Combine(Application.persistentDataPath, "maze_save_medium.json");

    public void SaveMazeState(Vector2Int playerPos)
    {
        MazeSaveDataMedium saveData = new MazeSaveDataMedium();
        saveData.numX = numX;
        saveData.numY = numY;
        saveData.playerX = playerPos.x;
        saveData.playerY = playerPos.y;

        var roomList = new List<MazeSaveDataMedium.RoomData>();

        for (int i = 0; i < numX; ++i)
        {
            for (int j = 0; j < numY; ++j)
            {
                Room r = rooms[i, j];
                var rd = new MazeSaveDataMedium.RoomData
                {
                    topWall = r.HasWall(Room.Directions.TOP),
                    rightWall = r.HasWall(Room.Directions.RIGHT),
                    bottomWall = r.HasWall(Room.Directions.BOTTOM),
                    leftWall = r.HasWall(Room.Directions.LEFT)
                };
                roomList.Add(rd);
            }
        }

        saveData.rooms = roomList.ToArray();
        File.WriteAllText(savePath, JsonUtility.ToJson(saveData));
    }

    public bool LoadMazeState()
    {
        if (!File.Exists(savePath)) return false;

        string json = File.ReadAllText(savePath);
        MazeSaveDataMedium saveData = JsonUtility.FromJson<MazeSaveDataMedium>(json);

        numX = saveData.numX;
        numY = saveData.numY;

        foreach (var room in rooms)
            if (room != null)
                Destroy(room.gameObject);

        rooms = new Room[numX, numY];

        for (int i = 0; i < numX; ++i)
        {
            for (int j = 0; j < numY; ++j)
            {
                float offsetX = (numX - 1) * roomWidth / 2f;
                float offsetY = (numY - 1) * roomHeight / 2f;
                Vector2 anchoredPos = new Vector2(i * roomWidth - offsetX, j * roomHeight - offsetY);

                GameObject room = Instantiate(roomPrefab, roomContainer);
                room.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
                room.name = $"Room_{i}_{j}";

                Room r = room.GetComponent<Room>();
                r.Index = new Vector2Int(i, j);
                rooms[i, j] = r;

                var data = saveData.rooms[i * numY + j];
                r.SetDirFlag(Room.Directions.TOP, data.topWall);
                r.SetDirFlag(Room.Directions.RIGHT, data.rightWall);
                r.SetDirFlag(Room.Directions.BOTTOM, data.bottomWall);
                r.SetDirFlag(Room.Directions.LEFT, data.leftWall);
            }
        }

        // Restore player
        if (playerInstance != null)
            Destroy(playerInstance);

        Vector3 spawnPos = rooms[saveData.playerX, saveData.playerY].transform.position;
        playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity, roomContainer);

        PlayerControllerMedium pc = playerInstance.GetComponent<PlayerControllerMedium>();
        if (pc != null)
        {
            pc.mazeGenerator = this;
            pc.roomWidth = roomWidth;
            pc.roomHeight = roomHeight;
            pc.SetRoomGrid(rooms);
            pc.blockingPanels = blockingPanels;
            pc.SetPlayerIndex(new Vector2Int(saveData.playerX, saveData.playerY));
        }

        yourMazeEndControllerReference.playerController = pc;

        // === Restore Invisible Exit Room ===
        GameObject invisibleExitRoom = new GameObject("InvisibleExitRoom");
        invisibleExitRoom.transform.SetParent(roomContainer, false);

        RectTransform invisibleRT = invisibleExitRoom.AddComponent<RectTransform>();
        invisibleRT.sizeDelta = new Vector2(roomWidth, roomHeight);

        float invisibleOffsetX = (numX - 1) * roomWidth / 2f;
        float invisibleOffsetY = (numY - 1) * roomHeight / 2f;
        Vector2 invisibleRoomPos = new Vector2(numX * roomWidth - invisibleOffsetX, (numY - 1) * roomHeight - invisibleOffsetY);
        invisibleRT.anchoredPosition = invisibleRoomPos;

        GameObject exitTriggerGO = new GameObject("FinalExitTrigger");
        exitTriggerGO.transform.SetParent(invisibleExitRoom.transform, false);

        RectTransform exitRT = exitTriggerGO.AddComponent<RectTransform>();
        exitRT.anchorMin = Vector2.zero;
        exitRT.anchorMax = Vector2.one;
        exitRT.offsetMin = Vector2.zero;
        exitRT.offsetMax = Vector2.zero;

        ExitTriggerMedium invisibleTrigger = exitTriggerGO.AddComponent<ExitTriggerMedium>();
        invisibleTrigger.mazeEndController = yourMazeEndControllerReference;

        BoxCollider2D invisibleCol = exitTriggerGO.AddComponent<BoxCollider2D>();
        invisibleCol.isTrigger = true;

        Image invisibleImg = exitTriggerGO.AddComponent<Image>();
        invisibleImg.color = new Color(0, 0, 0, 0); // fully transparent

        return true;
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
            File.Delete(savePath);
    }

    private void Reset()
    {
        for(int i = 0; i < numX; ++i)
        {
            for(int j = 0; j < numY; ++j)
            {
                rooms[i, j].SetDirFlag(Room.Directions.TOP, true);
                rooms[i, j].SetDirFlag(Room.Directions.RIGHT, true);
                rooms[i, j].SetDirFlag(Room.Directions.BOTTOM, true);
                rooms[i, j].SetDirFlag(Room.Directions.LEFT, true);
                rooms[i, j].visited = false;
            }
        }
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!generating)
            {
                CreateMaze();
            }
        }
    }*/
}
