using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public enum DoorPosition
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

enum LobbyDoor{
    NORMAL,
    NEXT_LEVEL,
    TUTORIAL,
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
            switch (lobbyDoor){
                case LobbyDoor.NORMAL:
                    Vector3 spawnPoint = linkedDoor.GetSpawnPoint();

                    other.transform.position = spawnPoint;

                    PlayerManager.Instance.GetMinions().position = spawnPoint;
                    foreach(Transform minion in PlayerManager.Instance.GetMinions()){
                        try{
                            minion.GetComponent<NavMeshAgent>().enabled = false;
                            minion.localPosition = Vector3.zero;
                            minion.GetComponent<NavMeshAgent>().enabled = true;
                        } catch {
                            minion.localPosition = Vector3.zero;
                        }
                    }
                    break;
                case LobbyDoor.NEXT_LEVEL:
                    GameManager.Instance.LoadNextLevel();
                    break;
                case LobbyDoor.TUTORIAL:
                    GameManager.Instance.LoadTutorial();
                    break;
                default:
                    break;
            }




            // if(lobbyDoor.Equals(LobbyDoor.NORMAL)){
            // } else {
            //     //TODO: Load new scene
            //     ManageLobbyDoor();
            // }
        }
    }

    // void ManageLobbyDoor(){
    //     switch(lobbyDoor){
    //         case LobbyDoor.NEXT_LEVEL:
    //             Debug.Log("Start Game");
    //             GameEvent.isInLobby = false;
    //             break;
    //         case LobbyDoor.TUTORIAL:
    //             Debug.Log("Tutorial");
    //             break;
    //         default:
    //             break;
    //     }
    // }
}
