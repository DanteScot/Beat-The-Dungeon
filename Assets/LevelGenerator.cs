using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    #region Generator Variables
    [SerializeField] int seed;

    [SerializeField] GameObject[] roomPrefab;
    [SerializeField] GameObject bossRoomPrefab;
    [SerializeField] private int minRooms = 10, maxRooms = 15;
    [SerializeField] int gridSizeX = 10, gridSizeY = 10;

    int roomWidth = 59, roomHeight = 32;
    private List<GameObject> roomObject = new List<GameObject>();
    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();
    private int[,] roomGrid;
    private int roomCount;
    private bool generationComplete = false;

    static Random.State state;
    #endregion


    private void Start() {
        if (seed < 0) seed *= -1;
        else if (seed == 0) seed = Random.Range(1, int.MaxValue);

        Random.InitState(seed);
        Debug.Log(seed);
        
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);


        StartCoroutine(FakeUpdate());
    }

    IEnumerator FakeUpdate(){
        while(!generationComplete){
            if(roomQueue.Count > 0 && roomCount < maxRooms) {
                Vector2Int currentRoomIndex = roomQueue.Dequeue();
                
                TryGenerateRoom(new Vector2Int(currentRoomIndex.x - 1, currentRoomIndex.y));
                TryGenerateRoom(new Vector2Int(currentRoomIndex.x + 1, currentRoomIndex.y));
                TryGenerateRoom(new Vector2Int(currentRoomIndex.x, currentRoomIndex.y - 1));
                TryGenerateRoom(new Vector2Int(currentRoomIndex.x, currentRoomIndex.y + 1));
            }
            else if(roomCount < minRooms) {
                Debug.Log("Too few rooms, regenerating");
                RegenerateRooms();
                break;
            } 
            else if (!generationComplete){
                if(!TryGenerateBossRoom()){
                    Debug.Log("Failed to generate boss room, regenerating");
                    RegenerateRooms();
                    break;
                }

                generationComplete = true;
                Debug.Log($"Generation complete with {roomCount} rooms");
                Messenger.Broadcast(GameEvent.LEVEL_GENERATED);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void StartRoomGenerationFromRoom(Vector2Int roomIndex) {
        roomQueue.Enqueue(roomIndex);
        roomGrid[roomIndex.x, roomIndex.y] = 1;
        roomCount++;
        var initialRoom = Instantiate(roomPrefab[0], GetPositionFromGridIndex(roomIndex), Quaternion.identity, transform);
        initialRoom.name = $"Room-{roomCount}";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex;
        roomObject.Add(initialRoom);
    }

    GameObject ChooseRoomPrefab(){
        GameObject chosenRoom = null;

        while(chosenRoom == null){
            foreach (var room in roomPrefab)
            {
                if(Random.value < .6f){
                    chosenRoom = room;
                    break;
                }
            }
        }

        return chosenRoom;
    }

    private bool TryGenerateRoom(Vector2Int roomIndex){
        if (roomCount >= maxRooms) return false;

        if (Random.value < .5f && roomIndex != Vector2Int.zero) return false;

        if (CountAdjacentRooms(roomIndex) > 1) return false;

        roomQueue.Enqueue(roomIndex);
        roomGrid[roomIndex.x, roomIndex.y] = 1;
        roomCount++;

        var newRoom = Instantiate(ChooseRoomPrefab(), GetPositionFromGridIndex(roomIndex), Quaternion.identity, transform);
        newRoom.name = newRoom.name.Replace("(Clone)", $" - {roomCount}");
        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        roomObject.Add(newRoom);

        CreateDoors(newRoom, roomIndex.x, roomIndex.y);

        return true;
    }

    private bool TryGenerateBossRoom(){
        Vector2Int validIndex = Vector2Int.zero;
        for(int i=1; i<roomObject.Count; i++){
            Vector2Int checkedIndex = roomObject[roomObject.Count - i].GetComponent<Room>().RoomIndex;

            if(CountAdjacentRooms(new Vector2Int(checkedIndex.x, checkedIndex.y+1)) == 1){
                validIndex = new Vector2Int(checkedIndex.x, checkedIndex.y+1);
                break;
            }
            else if(CountAdjacentRooms(new Vector2Int(checkedIndex.x, checkedIndex.y-1)) == 1){
                validIndex = new Vector2Int(checkedIndex.x, checkedIndex.y-1);
                break;
            }
            else if(CountAdjacentRooms(new Vector2Int(checkedIndex.x+1, checkedIndex.y)) == 1){
                validIndex = new Vector2Int(checkedIndex.x+1, checkedIndex.y);
                break;
            }
            else if(CountAdjacentRooms(new Vector2Int(checkedIndex.x-1, checkedIndex.y)) == 1){
                validIndex = new Vector2Int(checkedIndex.x-1, checkedIndex.y);
                break;
            }
        }

        if(validIndex == Vector2Int.zero) return false;
        
        roomQueue.Enqueue(validIndex);
        roomGrid[validIndex.x, validIndex.y] = 1;
        roomCount++;

        var newRoom = Instantiate(bossRoomPrefab, GetPositionFromGridIndex(validIndex), Quaternion.identity, transform);
        newRoom.name = newRoom.name.Replace("(Clone)", $" - {roomCount}");
        newRoom.GetComponent<Room>().RoomIndex = validIndex;
        roomObject.Add(newRoom);

        CreateDoors(newRoom, validIndex.x, validIndex.y);

        return true;
    }

    public void RegenerateRooms(){
        generationComplete = false;
        roomObject.ForEach(Destroy);
        roomObject.Clear();
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);

        StartCoroutine(FakeUpdate());
    }

    void CreateDoors(GameObject room, int x, int y){
        Room newRoomScript = room.GetComponent<Room>();

        Room leftRoomScript = GetRoomScriptAt(new Vector2Int(x - 1, y));
        Room rightRoomScript = GetRoomScriptAt(new Vector2Int(x + 1, y));
        Room topRoomScript = GetRoomScriptAt(new Vector2Int(x, y + 1));
        Room bottomRoomScript = GetRoomScriptAt(new Vector2Int(x, y - 1));

        if (leftRoomScript != null) {
            newRoomScript.CreateDoor(Vector2Int.left, leftRoomScript);
            leftRoomScript.CreateDoor(Vector2Int.right, newRoomScript);
        }
        if (rightRoomScript != null) {
            newRoomScript.CreateDoor(Vector2Int.right, rightRoomScript);
            rightRoomScript.CreateDoor(Vector2Int.left, newRoomScript);
        }
        if (topRoomScript != null) {
            newRoomScript.CreateDoor(Vector2Int.up, topRoomScript);
            topRoomScript.CreateDoor(Vector2Int.down, newRoomScript);
        }
        if (bottomRoomScript != null) {
            newRoomScript.CreateDoor(Vector2Int.down, bottomRoomScript);
            bottomRoomScript.CreateDoor(Vector2Int.up, newRoomScript);
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
