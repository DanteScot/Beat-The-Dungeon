using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject topDoor, bottomDoor, leftDoor, rightDoor;
    [SerializeField] GameObject enemyPrefab;

    public Vector2Int RoomIndex { get; set; }

    private void Start() {
        // if(Random.Range(0, 100) < 25){
        //     Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        // }
    }

    public void CreateDoor(Vector2Int direction, Room otherRoom){
        if(direction == Vector2Int.up){
            topDoor.SetActive(true);
            topDoor.GetComponent<DoorController>().linkedDoor = otherRoom.GetDoor(Vector2Int.down);
        }
        else if(direction == Vector2Int.down){
            bottomDoor.SetActive(true);
            bottomDoor.GetComponent<DoorController>().linkedDoor = otherRoom.GetDoor(Vector2Int.up);
        }
        else if(direction == Vector2Int.left){
            leftDoor.SetActive(true);
            leftDoor.GetComponent<DoorController>().linkedDoor = otherRoom.GetDoor(Vector2Int.right);
        }
        else if(direction == Vector2Int.right){
            rightDoor.SetActive(true);
            rightDoor.GetComponent<DoorController>().linkedDoor = otherRoom.GetDoor(Vector2Int.left);
        }
    }

    public DoorController GetDoor(Vector2Int direction){
        if(direction == Vector2Int.up){
            return topDoor.GetComponent<DoorController>();
        }
        else if(direction == Vector2Int.down){
            return bottomDoor.GetComponent<DoorController>();
        }
        else if(direction == Vector2Int.left){
            return leftDoor.GetComponent<DoorController>();
        }
        else if(direction == Vector2Int.right){
            return rightDoor.GetComponent<DoorController>();
        }
        return null;
    }

    private void OnDestroy() {
        foreach(Transform child in transform){
            Destroy(child.gameObject);
        }
    }
}
