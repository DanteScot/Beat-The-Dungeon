using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] int seed;

    [SerializeField] GameObject roomPrefab;
    [SerializeField] private int minRooms = 10, maxRooms = 15;

    int roomWidth = 59, roomHeight = 32;

    int gridSizeX = 10, gridSizeY = 10;

    private List<GameObject> roomObject = new List<GameObject>();

    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();

    private int[,] roomGrid;

    private int roomCount;

    private bool generationComplete = false;

    private void Start() {
        Debug.Log(int.MaxValue);
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);

        Random.InitState(seed);
    }

    private void Update() {
        if(roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete) {
            Vector2Int currentRoomIndex = roomQueue.Dequeue();
            
            TryGenerateRoom(new Vector2Int(currentRoomIndex.x - 1, currentRoomIndex.y));
            TryGenerateRoom(new Vector2Int(currentRoomIndex.x + 1, currentRoomIndex.y));
            TryGenerateRoom(new Vector2Int(currentRoomIndex.x, currentRoomIndex.y - 1));
            TryGenerateRoom(new Vector2Int(currentRoomIndex.x, currentRoomIndex.y + 1));
        }
        else if(roomCount < minRooms) {
            Debug.Log("Too few rooms, regenerating");
            RegenerateRooms();
        } 
        else if (!generationComplete){
            Debug.Log($"Generation complete with {roomCount} rooms");
            generationComplete = true;
        }
    }

    private void StartRoomGenerationFromRoom(Vector2Int roomIndex) {
        roomQueue.Enqueue(roomIndex);
        roomGrid[roomIndex.x, roomIndex.y] = 1;
        roomCount++;
        var initialRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        initialRoom.name = $"Room-{roomCount}";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex;
        roomObject.Add(initialRoom);
    }

    private bool TryGenerateRoom(Vector2Int roomIndex){
        if (roomCount >= maxRooms) return false;

        if (Random.value < .5f && roomIndex != Vector2Int.zero) return false;

        if (CountAdjacentRooms(roomIndex) > 1) return false;

        roomQueue.Enqueue(roomIndex);
        roomGrid[roomIndex.x, roomIndex.y] = 1;
        roomCount++;

        var newRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        newRoom.name = $"Room-{roomCount}";
        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        roomObject.Add(newRoom);

        CreateDoors(newRoom, roomIndex.x, roomIndex.y);

        return true;
    }

    public void RegenerateRooms(){
        roomObject.ForEach(Destroy);
        roomObject.Clear();
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    void CreateDoors(GameObject room, int x, int y){
        Room newRoomScript = room.GetComponent<Room>();

        Room leftRoomScript = GetRoomScriptAt(new Vector2Int(x - 1, y));
        Room rightRoomScript = GetRoomScriptAt(new Vector2Int(x + 1, y));
        Room topRoomScript = GetRoomScriptAt(new Vector2Int(x, y + 1));
        Room bottomRoomScript = GetRoomScriptAt(new Vector2Int(x, y - 1));

        if (leftRoomScript != null) {
            newRoomScript.OpenDoor(Vector2Int.left);
            leftRoomScript.OpenDoor(Vector2Int.right);
        }
        if (rightRoomScript != null) {
            newRoomScript.OpenDoor(Vector2Int.right);
            rightRoomScript.OpenDoor(Vector2Int.left);
        }
        if (topRoomScript != null) {
            newRoomScript.OpenDoor(Vector2Int.up);
            topRoomScript.OpenDoor(Vector2Int.down);
        }
        if (bottomRoomScript != null) {
            newRoomScript.OpenDoor(Vector2Int.down);
            bottomRoomScript.OpenDoor(Vector2Int.up);
        }
    }

    Room GetRoomScriptAt(Vector2Int roomIndex){
        GameObject room = roomObject.Find(r => r.GetComponent<Room>().RoomIndex == roomIndex);
        return room == null ? null : room.GetComponent<Room>();
    }

    private int CountAdjacentRooms(Vector2Int roomIndex) {
        int count = 0;

        if (roomIndex.x > 0 && roomGrid[roomIndex.x - 1, roomIndex.y] != 0) count++;
        if (roomIndex.x < gridSizeX - 1 && roomGrid[roomIndex.x + 1, roomIndex.y] != 0) count++;
        if (roomIndex.y > 0 && roomGrid[roomIndex.x, roomIndex.y - 1] != 0) count++;
        if (roomIndex.y < gridSizeY - 1 && roomGrid[roomIndex.x, roomIndex.y + 1] != 0) count++;

        return count;
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex) {
        return new Vector3(roomWidth * (gridIndex.x - gridSizeX / 2), roomHeight * (gridIndex.y - gridSizeY / 2));
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(x, y));
                Gizmos.DrawWireCube(position, new Vector3(roomWidth, roomHeight, 0));
            }
        }
    }
}
