using UnityEngine;
using UnityEngine.AI;


enum DoorType{
    NORMAL,
    NEXT_LEVEL,
    TUTORIAL,
    LOBBY
}

// Classe che gestisce il comportamento delle porte
[RequireComponent(typeof(BoxCollider2D))]
public class DoorController : MonoBehaviour
{
    [SerializeField] private Sprite closedDoor;
    [SerializeField] private Sprite openDoor;
    [SerializeField] private DoorController linkedDoor;

    [SerializeField] private bool isOpen = true;

    // Gestisce diversamente il comportamento della porta se si trova in lobby
    [SerializeField] private DoorType doorType;

    public void OpenDoor(){
        GetComponent<SpriteRenderer>().sprite = openDoor;
        isOpen = true;
    }

    public void CloseDoor(){
        GetComponent<SpriteRenderer>().sprite = closedDoor;
        isOpen = false;
    }

    // Ritorna il punto in cui il player deve spawnare quando entra in una porta
    public Vector3 GetSpawnPoint(){
        return transform.position+(transform.up*-2);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && isOpen)
        {
            switch (doorType){
                case DoorType.NORMAL:
                    Vector3 spawnPoint = linkedDoor.GetSpawnPoint();

                    other.transform.position = spawnPoint;

                    // Se il player ha dei minion, li fa spawnare al suo fianco
                    // Alcuni minion potrebbero non avere il NavMeshAgent, quindi li sposta manualmente
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
                case DoorType.NEXT_LEVEL:
                    GameManager.Instance.LoadNextLevel();
                    break;
                case DoorType.TUTORIAL:
                    GameManager.Instance.LoadTutorial();
                    break;
                case DoorType.LOBBY:
                    GameManager.Instance.LoadLobby();
                    break;
                default:
                    break;
            }
        }
    }
}
