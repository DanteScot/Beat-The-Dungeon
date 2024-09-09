using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private GameObject[] rewardPrefab;
    [SerializeField] private bool isLobbyRoom=false;
    public Collider2D[] enemies { get; private set; }
    
    /// <summary>
    /// This variable is used to check if the room is active or not
    /// room is active when an enemy is inside the room
    /// only active rooms can drop items
    /// </summary>
    private bool isRoomActive;

    private TilemapCollider2D tilemapCollider;

    private float roomX;
    private float roomY;
    private Vector3 roomCenter;

    private DoorController[] doors;

    void Awake()
    {
        var tilemaps = transform.parent.GetComponentsInChildren<Tilemap>();
        foreach (var tilemap in tilemaps)
        {
            tilemap.CompressBounds();
        }

        tilemapCollider = transform.parent.GetComponentInChildren<TilemapCollider2D>();
    }

    void Start()
    {
        roomX=tilemapCollider.bounds.size.x;
        roomY=tilemapCollider.bounds.size.y;
        roomCenter = tilemapCollider.bounds.center;

        isRoomActive = false;

        FindEnemies();
        if(enemies.Length>0) isRoomActive = true;

        doors = transform.parent.GetComponentsInChildren<DoorController>();
        foreach (var door in doors)
        {
            if(isRoomActive)    door.CloseDoor();
            else                door.OpenDoor();
        }
    }






    // TODO
    // TEST CODE
    // MUST BE REMOVED
    //
    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            var hit = Physics2D.OverlapBoxAll(roomCenter, new Vector2(roomX, roomY), 0);
            
            foreach (var player in hit)
            {
                if(player.CompareTag("Player")){
                    foreach (var item in hit)
                    {
                        if(item.CompareTag("Enemy")){
                            item.GetComponent<Enemy>().TakeDamage(100);
                        }
                    }

                    break;
                }
            }
        }
    }








    void LateUpdate()
    {
        if(isRoomActive){
            CheckRoom();
        }
    }

    // void CheckRoom(){
    //     bool found = false;
    //     var hit = Physics2D.OverlapBoxAll(roomCenter, new Vector2(roomX, roomY), 0);
    //     foreach (var item in hit)
    //     {
    //         if(item.CompareTag("Enemy")){
    //             found = true;
    //             break;
    //         }
    //     }
    //     if(!found){
    //         RoomCleared();
    //     }
    // }
    void FindEnemies(){
        enemies = Physics2D.OverlapBoxAll(roomCenter, new Vector2(roomX, roomY), 0, LayerMask.GetMask("Enemy"));
    }

    void CheckRoom(){
        bool found = false;

        FindEnemies();
        if(enemies.Length>0) found = true;

        if(!found) RoomCleared();
    }

    void RoomCleared(){
        isRoomActive = false;
        if(!isLobbyRoom) Instantiate(rewardPrefab[Random.Range(0,rewardPrefab.Length)], roomCenter, Quaternion.identity);
        foreach (var door in doors)
        {
            door.OpenDoor();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")){
            FindEnemies();
            foreach (var enemy in enemies)
            {
                // Debug.Log(enemy.name);
                if(!isLobbyRoom) enemy.GetComponent<Enemy>().isActive = true;
            }
            other.GetComponent<PlayerController>().SetCurrentRoom(this);
            PlayerManager.Instance.currentRoom = this;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player")){
            FindEnemies();
            foreach (var enemy in enemies)
            {
                if(!isLobbyRoom) enemy.GetComponent<Enemy>().isActive = false;
            }
        }
    }
}
