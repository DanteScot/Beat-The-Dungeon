using UnityEngine;
using UnityEngine.Tilemaps;

// Classe che si occupa di gestire le stanze del gioco
public class LobbyController : MonoBehaviour
{
    public Collider2D[] enemies { get; private set; }
    
    private bool isRoomActive;

    public TilemapCollider2D TilemapCollider { get; private set; }

    private float roomX;
    private float roomY;
    private Vector3 roomCenter;

    [SerializeField] private DoorController[] doors;

    void Awake()
    {
        // Comprime i bordi dei tilemap per evitare che risultino più grandi di quanto siano realmente
        var tilemaps = transform.parent.GetComponentsInChildren<Tilemap>();
        foreach (var tilemap in tilemaps)
        {
            tilemap.CompressBounds();
        }

        TilemapCollider = transform.parent.GetComponentInChildren<TilemapCollider2D>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Start()
    {
        // Calcola le dimensioni della stanza ed il suo centro
        roomX=TilemapCollider.bounds.size.x;
        roomY=TilemapCollider.bounds.size.y;
        roomCenter = TilemapCollider.bounds.center;

        isRoomActive = false;

        FindEnemies();
        if(enemies.Length>0) isRoomActive = true;

        foreach (var door in doors)
        {
            if(isRoomActive)    door.CloseDoor();
            else                door.OpenDoor();
        }
    }

    // Finchè la stanza è attiva controlla se ci sono nemici al suo interno, se non ce ne sono più la stanza è stata completata
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

    // Istanzia il premio e apre le porte della stanza
    void RoomCleared(){
        isRoomActive = false;
        foreach (var door in doors)
        {
            door.OpenDoor();
        }
    }

    // Quando il giocatore entra nella stanza, si imposta come stanza corrente e si attivano i nemici
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")){
            other.GetComponent<PlayerController>().SetCurrentRoom(this);
            // PlayerManager.Instance.currentRoom = this;
        }
    }
}
