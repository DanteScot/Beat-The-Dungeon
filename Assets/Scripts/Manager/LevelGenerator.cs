using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour
{
    #region Generator Variables
    [SerializeField] int seed;
    [SerializeField] GameObject[] roomPrefab;
    
    private int minRooms, maxRooms;
    int gridSizeX = 10, gridSizeY = 10;
    private readonly int roomWidth = 59;
    private readonly int roomHeight = 32;

    private List<GameObject> roomObject = new();
    private Queue<Vector2Int> roomQueue = new();
    private int[,] roomGrid;
    private int roomCount;
    private static Random.State state;
    #endregion

    [SerializeField] Sprite[] loadingSprites;
    GameObject loadingScreen;

    private bool generationComplete = false;
    int roomGenerated;

    Coroutine loadingCoroutine;

    private void Awake() {
        Messenger.AddListener(GameEvent.ROOM_GENERATED, OnRoomGenerated);

        loadingScreen = transform.GetChild(0).gameObject;
        loadingScreen.SetActive(true);
        loadingCoroutine = StartCoroutine(LoadingAnimation());
    }

    private void Start() {
        BeatManager.Instance.AudioSource.Stop();
        GameEvent.canMove = false;
        seed = GameManager.Instance.seed;
        
        
        if (GameManager.Instance.GetLevel() <= 1){
            if (seed < 0) seed *= -1;
            else if (seed == 0) seed = Random.Range(1, int.MaxValue);

            Random.InitState(seed);
        } else {
            Random.state = state;
        }

        // if (seed == 0) seed = Random.Range(1, int.MaxValue);
        // Random.InitState(seed);
        
        minRooms = (int)(5 + 1.5f*GameManager.Instance.GetLevel());
        maxRooms = minRooms + minRooms/2;
        gridSizeX = minRooms;
        gridSizeY = minRooms;

        roomGenerated = 0;

        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();

        Vector2Int initialRoomIndex = new(gridSizeX / 2, gridSizeY / 2);
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
        GameObject initialRoom = Instantiate(roomPrefab[0], GetPositionFromGridIndex(roomIndex), Quaternion.identity, transform);
        initialRoom.name = $"Room - {roomCount}";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex;
        roomObject.Add(initialRoom);
    }

    GameObject ChooseRoomPrefab(){
        GameObject chosenRoom = null;

        while(chosenRoom == null){
            foreach (GameObject room in roomPrefab)
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
        try{
            if (roomCount >= maxRooms || roomGrid[roomIndex.x, roomIndex.y] == 1) return false;

            if (Random.value < .5f && roomIndex != Vector2Int.zero) return false;

            if (CountAdjacentRooms(roomIndex) > 1) return false;

            roomQueue.Enqueue(roomIndex);
            roomGrid[roomIndex.x, roomIndex.y] = 1;
            roomCount++;

            GameObject newRoom = Instantiate(ChooseRoomPrefab(), GetPositionFromGridIndex(roomIndex), Quaternion.identity, transform);
            newRoom.name = newRoom.name.Replace("(Clone)", $" - {roomCount}");
            newRoom.GetComponent<Room>().RoomIndex = roomIndex;
            roomObject.Add(newRoom);

            CreateDoors(newRoom, roomIndex.x, roomIndex.y);

            return true;
        } catch {
            return false;
        }
    }

    private bool TryGenerateBossRoom(){
        try{
            Vector2Int validIndex = Vector2Int.zero;
            for(int i=1; i<roomObject.Count; i++){
                Vector2Int checkedIndex = roomObject[^i].GetComponent<Room>().RoomIndex;

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

            GameObject newRoom = Instantiate(ChooseRoomPrefab(), GetPositionFromGridIndex(validIndex), Quaternion.identity, transform);
            newRoom.name = newRoom.name = $"BossRoom - {roomCount}";
            newRoom.GetComponent<Room>().RoomIndex = validIndex;
            roomObject.Add(newRoom);

            CreateDoors(newRoom, validIndex.x, validIndex.y);

            return true;
        } catch {
            return false;
        }
    }

    public void RegenerateRooms(){
        generationComplete = false;
        roomObject.ForEach(Destroy);
        roomObject.Clear();
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;

        Vector2Int initialRoomIndex = new(gridSizeX / 2, gridSizeY / 2);
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

    private void OnRoomGenerated(){
        roomGenerated++;

        if(roomGenerated == roomCount){
            StopCoroutine(loadingCoroutine);
            Destroy(loadingScreen);
            state = Random.state;
            GameEvent.canMove = true;
            BeatManager.Instance.AudioSource.Play();
        }
    }

    IEnumerator LoadingAnimation(){
        Image image = loadingScreen.transform.GetChild(1).GetComponent<Image>();
        while (true){
            for(int i=0; i<loadingSprites.Length; i++){
                try{
                    image.sprite = loadingSprites[i];
                } catch {
                    break;
                }
                yield return new WaitForSeconds(.25f);
            }
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

    private void OnDestroy() {
        Messenger.RemoveListener(GameEvent.ROOM_GENERATED, OnRoomGenerated);
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
