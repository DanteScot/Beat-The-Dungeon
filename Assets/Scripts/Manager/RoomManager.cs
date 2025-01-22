using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.Tilemaps;

// Classe che si occupa di gestire le stanze del gioco
public class RoomManager : MonoBehaviour
{
    // Array contenente i possibili premi che possono essere droppati
    [SerializeField] private GameObject[] rewardPrefab;
    GameObject reward;
    [SerializeField] private bool isLobbyRoom=false;
    public Collider2D[] enemies { get; private set; }
    
    /// <summary>
    /// This variable is used to check if the room is active or not
    /// room is active when an enemy is inside the room
    /// only active rooms can drop items
    /// </summary>
    [SerializeField] private bool isRoomActive;

    private TilemapCollider2D tilemapCollider;

    private float roomX;
    private float roomY;
    private Vector3 roomCenter;

    [SerializeField] private DoorController[] doors;

    List<NavMeshSurface> navMeshes;

    Transform content;

    void Awake()
    {
        // Comprime i bordi dei tilemap per evitare che risultino più grandi di quanto siano realmente
        var tilemaps = transform.parent.GetComponentsInChildren<Tilemap>();
        foreach (var tilemap in tilemaps)
        {
            tilemap.CompressBounds();
        }

        tilemapCollider = transform.parent.GetComponentInChildren<TilemapCollider2D>();

        if(isLobbyRoom) return;

        navMeshes = new List<NavMeshSurface>();

        Messenger.AddListener(GameEvent.LEVEL_GENERATED, OnLevelGenerated);
    }

    void Start()
    {
        // Calcola le dimensioni della stanza ed il suo centro
        roomX=tilemapCollider.bounds.size.x;
        roomY=tilemapCollider.bounds.size.y;
        roomCenter = tilemapCollider.bounds.center;

        isRoomActive = false;

        content = transform.parent.Find("Content");

        foreach(GameObject obj in Resources.LoadAll<GameObject>("Prefabs/NavMeshes")){
            navMeshes.Add(Instantiate(obj, transform.parent).GetComponent<NavMeshSurface>());
            navMeshes[navMeshes.Count-1].transform.position = roomCenter;
            navMeshes[navMeshes.Count-1].size = new Vector3(roomX-2, 1, roomY-2);
        }
    }

    void OnLevelGenerated(){
        try{
            if (this == null || gameObject == null || !gameObject.activeSelf || !enabled) return;

            if(!transform.parent.name.Equals("Room-1") && !isLobbyRoom && !transform.parent.name.Contains("BossRoom"))
            {
                GameObject[] contents = Resources.LoadAll<GameObject>($"Prefabs/RoomContents/{transform.parent.name.Split(' ')[0]}");

                if(contents.Length>0) Instantiate(contents[Random.Range(0, contents.Length)], content);

                GameObject[] enemies = Resources.LoadAll<GameObject>("Prefabs/Enemy/Common");

                int totalGroups = System.Enum.GetValues(typeof(GenerationGroup)).Length;
                GameObject[] enemiesForGroup = new GameObject[totalGroups];
                for (int i = 0; i < totalGroups; i++)
                {
                    enemiesForGroup[i] = enemies[Random.Range(0, enemies.Length)];
                }

                if(navMeshes.Count>0)
                {
                    foreach (var nav in navMeshes)
                    {
                        nav.BuildNavMesh();
                    }
                }

                foreach (var spawner in content.GetComponentsInChildren<EnemySpawpoint>())
                {
                    // if (NavMesh.SamplePosition(spawner.transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                    // {
                    //     // Posiziona l'agente sul punto valido trovato
                    //     GameObject agent = Instantiate(enemiesForGroup[(int)spawner.generationGroup], hit.position, Quaternion.identity, content);
                    // }
                    Instantiate(enemiesForGroup[(int)spawner.generationGroup], spawner.transform.position, Quaternion.identity, content);
                }
            }

            StartCoroutine(WaitBeforeCheck());
            
        } catch (System.Exception ex) {
            Debug.LogError($"Error in RoomManager: {ex.Message}\n{ex.StackTrace}");
        }
    }


    IEnumerator WaitBeforeCheck(){
        yield return new WaitForSeconds(.05f);

        FindEnemies();
        if(enemies.Length>0) isRoomActive = true;

        List<string> requiredMeshes = new List<string>();
        foreach (var enemy in enemies)
        {
            requiredMeshes.Add(enemy.transform.GetComponent<Enemy>().requiredNavMesh.ToString());
        }

        List<NavMeshSurface> tmp = new List<NavMeshSurface>();
        foreach (var nav in navMeshes)
        {
            // if(!requiredMeshes.Contains(nav.name.Replace("(Clone)",""))) nav.gameObject.SetActive(false);
            if(!requiredMeshes.Contains(nav.name.Replace("(Clone)",""))){
                Destroy(nav.gameObject);
            } else {
                tmp.Add(nav);
            }
        }
        navMeshes = tmp;

        if(isRoomActive && !isLobbyRoom){
            if(Random.Range(0+PlayerManager.Instance.LuckLevelled*5, 100)>50) SetReward(rewardPrefab[Random.Range(0, rewardPrefab.Length)]);
        }

        foreach (var door in doors)
        {
            if(isRoomActive)    door.CloseDoor();
            else                door.OpenDoor();
        }
    }

    public void SetReward(GameObject reward){
        this.reward = Instantiate(reward, content.GetChild(0).Find("Reward"));;
    }



    
    // TODO: Da rimuovere in produzione
    // permette di eliminare tutti i nemici nella stanza con la barra spaziatrice, utile per testare cose
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
                            else item.GetComponent<Enemy>().TakeDamage(100);
                        }
                    }

                    break;
                }
            }
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
        if(reward) reward.SetActive(true);
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
            PlayerManager.Instance.currentRoom = this;

            if(isLobbyRoom) return;
            
            FindEnemies();

            if(reward && enemies.Length>0) reward.SetActive(false);

            foreach (var enemy in enemies)
            {
                enemy.GetComponent<Enemy>().isActive = true;
            }
        }
    }

    // Quando il giocatore esce dalla stanza, si disattivano i nemici
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

    private void OnDestroy() {
        Messenger.RemoveListener(GameEvent.LEVEL_GENERATED, OnLevelGenerated);

        StopAllCoroutines();
    }
}
