using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    /// <summary>
    /// This variable is used to check if the room is active or not
    /// room is active when an enemy is inside the room
    /// only active rooms can drop items
    /// </summary>
    [SerializeField] private bool isRoomActive;
    [SerializeField] private GameObject[] rewardPrefab;

    private TilemapCollider2D tilemapCollider;

    private float roomX;
    private float roomY;
    private Vector3 roomCenter;

    void Start()
    {
        tilemapCollider = transform.parent.GetComponentInChildren<TilemapCollider2D>();
        roomX=tilemapCollider.bounds.size.x;
        roomY=tilemapCollider.bounds.size.y;
        roomCenter = tilemapCollider.bounds.center;
    }

    void Update()
    {
        //TODO: door check

        if(isRoomActive){
            CheckRoom();
        }
    }

    void CheckRoom(){
        var hit = Physics2D.OverlapBox(roomCenter, new Vector2(roomX, roomY), 0, LayerMask.GetMask("Enemy"));
        if(hit == null){
            RoomCleared();
        }
    }

    void RoomCleared(){
        isRoomActive = false;
        Instantiate(rewardPrefab[Random.Range(0,rewardPrefab.Length)], roomCenter, Quaternion.identity);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")){
            Debug.Log("Player entered room");
            other.GetComponent<PlayerController>().SetCurrentRoom(this);
        }
    }
}
