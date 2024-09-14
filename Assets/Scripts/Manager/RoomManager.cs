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
                            if(item.name.Contains("Jerry"))  return;
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

    void FindEnemies(){
        enemies = Physics2D.OverlapBoxAll(roomCenter, new Vector2(roomX, roomY), 0, LayerMask.GetMask("Enemy"));
    }

    void CheckRoom(){
        FindEnemies();
        if(enemies.Length==0) RoomCleared();
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
            other.GetComponent<PlayerController>().SetCurrentRoom(this);
            PlayerManager.Instance.currentRoom = this;

            if(isLobbyRoom) return;

            foreach (var enemy in enemies)
            {
                enemy.GetComponent<Enemy>().isActive = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(isLobbyRoom) return;
        
        if(other.CompareTag("Player")){
            foreach (var enemy in enemies)
            {
                enemy.GetComponent<Enemy>().isActive = false;
            }
        }
    }
}
