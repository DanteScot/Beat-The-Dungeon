using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct RoomItem
{
    public GameObject itemPrefab;
    public ItemSelected itemSelected;
}

public class RoomManager : MonoBehaviour
{
    /// <summary>
    /// This variable is used to check if the room is active or not
    /// room is active when an enemy is inside the room
    /// only active rooms can drop items
    /// </summary>
    [SerializeField] private bool isRoomActive;
    [SerializeField] private RoomItem roomItem;

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
        Instantiate(roomItem.itemPrefab, roomCenter, Quaternion.identity).GetComponent<Items>().SetItem(roomItem.itemSelected);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")){
            other.GetComponent<PlayerController>().SetCurrentRoom(this);
        }
    }
}
