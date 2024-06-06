using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private GameObject[] rewardPrefab;
    
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

    void Start()
    {
        tilemapCollider = transform.parent.GetComponentInChildren<TilemapCollider2D>();
        roomX=tilemapCollider.bounds.size.x;
        roomY=tilemapCollider.bounds.size.y;
        roomCenter = tilemapCollider.bounds.center;

        isRoomActive = false;

        var hit = Physics2D.OverlapBoxAll(roomCenter, new Vector2(roomX, roomY), 0);
        foreach (var item in hit)
        {
            if(item.CompareTag("Enemy")){
                isRoomActive = true;
                break;
            }
        }

        doors = transform.parent.GetComponentsInChildren<DoorController>();
        foreach (var door in doors)
        {
            if(isRoomActive)    door.CloseDoor();
            else                door.OpenDoor();
        }
    }






    //
    // TEST CODE
    // MUST BE REMOVED
    //
    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            var hit = Physics2D.OverlapBoxAll(roomCenter, new Vector2(roomX, roomY), 0);
            foreach (var item in hit)
            {
                if(item.CompareTag("Enemy")){
                    item.GetComponent<Enemy>().TakeDamage(100);
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

    void CheckRoom(){
        bool found = false;
        var hit = Physics2D.OverlapBoxAll(roomCenter, new Vector2(roomX, roomY), 0);
        foreach (var item in hit)
        {
            if(item.CompareTag("Enemy")){
                found = true;
                break;
            }
        }
        if(!found){
            RoomCleared();
        }
    }

    void RoomCleared(){
        isRoomActive = false;
        Instantiate(rewardPrefab[Random.Range(0,rewardPrefab.Length)], roomCenter, Quaternion.identity);
        foreach (var door in doors)
        {
            door.OpenDoor();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")){
            var hit = Physics2D.OverlapBoxAll(roomCenter, new Vector2(roomX, roomY), 0);
            foreach (var enemy in hit)
            {
                if(enemy.CompareTag("Enemy"))
                {
                    Debug.Log(enemy.name);
                    enemy.GetComponent<Enemy>().isActive = true;
                }
            }
            other.GetComponent<PlayerController>().SetCurrentRoom(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player")){
            var hit = Physics2D.OverlapBoxAll(roomCenter, new Vector2(roomX, roomY), 0);
            foreach (var enemy in hit)
            {
                if(enemy.CompareTag("Enemy"))
                {
                    enemy.GetComponent<Enemy>().isActive = false;
                }
            }
        }
    }
}
