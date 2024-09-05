using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum DoorPosition
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

enum LobbyDoor{
    NOT_LOBBY,
    START_GAME,
    TUTORIAL
}


[RequireComponent(typeof(BoxCollider2D))]
public class DoorController : MonoBehaviour
{
    [SerializeField] private Sprite closedDoor;
    [SerializeField] private Sprite openDoor;
    [SerializeField] private DoorController linkedDoor;

    private bool isOpen = true;

    [SerializeField] private LobbyDoor lobbyDoor;

    public void OpenDoor(){
        GetComponent<SpriteRenderer>().sprite = openDoor;
        isOpen = true;
    }

    public void CloseDoor(){
        GetComponent<SpriteRenderer>().sprite = closedDoor;
        isOpen = false;
    }

    public Vector3 GetSpawnPoint(){
        return transform.position+(transform.up*-2);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && isOpen)
        {
            if(!lobbyDoor.Equals(LobbyDoor.NOT_LOBBY)){
                //TODO: Load new scene
                ManageLobbyDoor();
            } else {
                other.transform.position = linkedDoor.GetSpawnPoint();
            }
        }
    }

    void ManageLobbyDoor(){
        switch(lobbyDoor){
            case LobbyDoor.START_GAME:
                Debug.Log("Start Game");
                GameEvent.isInLobby = false;
                break;
            case LobbyDoor.TUTORIAL:
                Debug.Log("Tutorial");
                break;
            default:
                break;
        }
    }
}
