using UnityEngine;

[System.Serializable]
public class MazeSaveDataMedium
{
    public int numX, numY;
    public RoomData[] rooms;
    public int playerX, playerY;

    [System.Serializable]
    public class RoomData
    {
        public bool topWall;
        public bool rightWall;
        public bool bottomWall;
        public bool leftWall;
    }
}
