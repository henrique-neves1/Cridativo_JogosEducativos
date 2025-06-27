using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerHard : MonoBehaviour
{
    public float moveSpeed = 200f;
    public float moveTime = 0.2f;
    //private Vector3 targetPos;
    private bool moving = false;

    public float roomWidth = 100f;
    public float roomHeight = 100f;

    public GameObject[] blockingPanels;

    private RectTransform rectTransform;

    private Vector2Int currentIndex = new Vector2Int(0, 0);
    private Vector3 targetPosition;

    private Room[,] roomGrid;

    private bool canMove = true;

    public GenerateMazeHard mazeGenerator;

    public void SetRoomGrid(Room[,] rooms)
    {
        roomGrid = rooms;
    }

    /*private void Start()
    {
        targetPos = transform.position;
    }*/

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (roomGrid == null) return;

        bool panelIsBlocking = false;
        foreach (var panel in blockingPanels)
        {
            if (panel != null && panel.activeInHierarchy)
            {
                panelIsBlocking = true;
                break;
            }
        }

        if (!canMove || panelIsBlocking) return;

        Vector2Int dir = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.UpArrow)) dir = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) dir = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) dir = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow)) dir = Vector2Int.right;

        if (dir != Vector2Int.zero)
        {
            TryMove(dir);
        }
    }

    void TryMove(Vector2Int dir)
    {
        Vector2Int newIndex = currentIndex + dir;

        int maxX = roomGrid.GetLength(0);
        int maxY = roomGrid.GetLength(1);

        bool isMovingToInvisibleExit =
            currentIndex.x == maxX - 1 && currentIndex.y == maxY - 1 &&
            dir == Vector2Int.right &&
            newIndex.x == maxX && newIndex.y == maxY - 1;

        if (isMovingToInvisibleExit)
        {
            currentIndex = newIndex;

            float offsetX = (maxX - 1) * roomWidth / 2f;
            float offsetY = (maxY - 1) * roomHeight / 2f;
            Vector3 invisibleRoomPos = new Vector3(
                maxX * roomWidth - offsetX,
                (maxY - 1) * roomHeight - offsetY,
                0f
            );

            rectTransform.anchoredPosition = invisibleRoomPos;

            if (mazeGenerator != null)
                mazeGenerator.SaveMazeState(currentIndex);

            return;
        }

        // Bounds check
        if (newIndex.x < 0 || newIndex.x >= roomGrid.GetLength(0) ||
            newIndex.y < 0 || newIndex.y >= roomGrid.GetLength(1))
            return;

        Room currentRoom = roomGrid[currentIndex.x, currentIndex.y];

        // Check wall in that direction
        if (dir == Vector2Int.up && currentRoom.HasWall(Room.Directions.TOP)) return;
        if (dir == Vector2Int.down && currentRoom.HasWall(Room.Directions.BOTTOM)) return;
        if (dir == Vector2Int.left && currentRoom.HasWall(Room.Directions.LEFT)) return;
        if (dir == Vector2Int.right && currentRoom.HasWall(Room.Directions.RIGHT)) return;

        // Move
        currentIndex = newIndex;
        targetPosition = roomGrid[newIndex.x, newIndex.y].transform.position;
        transform.position = targetPosition;

        if (mazeGenerator != null)
            mazeGenerator.SaveMazeState(currentIndex);
    }

    public void SetPlayerIndex(Vector2Int index)
    {
        currentIndex = index;
    }

    public void DisableControl()
    {
        canMove = false;
    }

    private System.Collections.IEnumerator MoveTo(Vector3 destination)
    {
        moving = true;
        float elapsed = 0f;
        Vector3 start = transform.position;

        while (elapsed < moveTime)
        {
            transform.position = Vector3.Lerp(start, destination, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = destination;
        //targetPos = destination;
        moving = false;
    }
}
