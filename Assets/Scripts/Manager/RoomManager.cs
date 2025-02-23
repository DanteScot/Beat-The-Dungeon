using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.Tilemaps;

// Classe che si occupa di gestire le stanze del gioco
public class RoomManager : MonoBehaviour
{
    // Array contenente i possibili premi che possono essere droppati
    [SerializeField] private GameObject[] rewardPrefab;
    GameObject reward;
    [SerializeField] private bool isBossRoom=false;
    [SerializeField] private bool isStartingRoom=false;
    public Enemy[] Enemies { get; private set; }
    
    /// <summary>
    /// This variable is used to check if the room is active or not
    /// room is active when an enemy is inside the room
    /// only active rooms can drop items
    /// </summary>
    [SerializeField] private bool isRoomActive;

    public TilemapCollider2D TilemapCollider { get; private set; }

    private float roomX;
    private float roomY;
    private Vector3 roomCenter;

    [SerializeField] private DoorController[] doors;
    [SerializeField] private GameObject exitDoor;

    List<NavMeshSurface> navMeshes;

    Transform content;
    static GameObject[] enemiesPrefab;

    int currentLoadingRoom = 0;
    int selfIndex=0;

    void Awake()
    {
        // Comprime i bordi dei tilemap per evitare che risultino più grandi di quanto siano realmente
        Tilemap[] tilemaps = transform.parent.GetComponentsInChildren<Tilemap>();
        foreach (Tilemap tilemap in tilemaps)
        {
            tilemap.CompressBounds();
        }

        TilemapCollider = transform.parent.GetComponentInChildren<TilemapCollider2D>();

        navMeshes = new List<NavMeshSurface>();

        if(enemiesPrefab == null) enemiesPrefab = Resources.LoadAll<GameObject>("Prefabs/Enemy/Common");

        Messenger.AddListener(GameEvent.LEVEL_GENERATED, OnRoomGenerated);
        Messenger.AddListener(GameEvent.ROOM_GENERATED, OnRoomGenerated);
    }

    void Start()
    {
        // Calcola le dimensioni della stanza ed il suo centro
        roomX=TilemapCollider.bounds.size.x;
        roomY=TilemapCollider.bounds.size.y;
        roomCenter = TilemapCollider.bounds.center;

        isRoomActive = false;

        content = transform.parent.Find("Content");

        foreach(GameObject obj in Resources.LoadAll<GameObject>("Prefabs/NavMeshes")){
            navMeshes.Add(Instantiate(obj, transform.parent).GetComponent<NavMeshSurface>());
            navMeshes[^1].gameObject.name = navMeshes[^1].gameObject.name.Replace("(Clone)", "");
            navMeshes[^1].transform.position = roomCenter;
            navMeshes[^1].size = new Vector3(roomX-2, 1, roomY-2);
        }
        selfIndex = int.Parse(transform.parent.gameObject.name.Split(" - ")[1]);


        StartCoroutine(OnLevelGenerated());
    }

    void OnRoomGenerated(){
        currentLoadingRoom++;
    }

    IEnumerator OnLevelGenerated(){
        GameObject[] contents = Resources.LoadAll<GameObject>($"Prefabs/RoomContents/{transform.parent.name.Split(' ')[0]}");

        GameObject[] enemiesForGroup = new GameObject[System.Enum.GetValues(typeof(GenerationGroup)).Length];
        for (int i = 0; i < enemiesForGroup.Length; i++)
        {
            enemiesForGroup[i] = enemiesPrefab[Random.Range(0, enemiesPrefab.Length)];
        }


        yield return new WaitUntil(() => currentLoadingRoom == selfIndex);

        if (this == null || gameObject == null || !gameObject.activeSelf || !enabled) yield break;

        
        if(transform.parent.name.Equals("Room - 1")) {
            isStartingRoom = true;
            StartCoroutine(WaitBeforeCheck());
            yield break;
        }


        isBossRoom = transform.parent.name.Contains("BossRoom");

        if (isBossRoom) {
            GameObject[] bossPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemy/Boss");

            if (contents.Length > 0) Instantiate(contents[Random.Range(0, contents.Length)], content);
            EnemySpawpoint spawner = content.GetComponentInChildren<EnemySpawpoint>();

            yield return null;

            if (navMeshes.Count > 0)
            {
                foreach (NavMeshSurface nav in navMeshes)
                {
                    nav.BuildNavMesh();
                    yield return null; // Dato che il navmesh è pesante, appena è generato mostra il frame senza aspettare ulteriormente (riduce rischio di freeze)
                }
            }

            Instantiate(bossPrefabs[Random.Range(0, bossPrefabs.Length)], spawner.transform.position, Quaternion.identity, content);
        } 
        else {
            if (contents.Length > 0) Instantiate(contents[Random.Range(0, contents.Length)], content);
            EnemySpawpoint[] spawners = content.GetComponentsInChildren<EnemySpawpoint>().Where(x => x.CompareTag("EnemySpawner")).ToArray();

            yield return null;

            if (navMeshes.Count > 0)
            {
                foreach (NavMeshSurface nav in navMeshes)
                {
                    nav.BuildNavMesh();
                    yield return null; // Dato che il navmesh è pesante, appena è generato mostra il frame senza aspettare ulteriormente (riduce rischio di freeze)
                }
            }

            for (int i = 0; i < spawners.Length; i++)
            {
                // if (NavMesh.SamplePosition(spawner.transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                // {
                //     // Posiziona l'agente sul punto valido trovato
                //     GameObject agent = Instantiate(enemiesForGroup[(int)spawner.generationGroup], hit.position, Quaternion.identity, content);
                // }
                Instantiate(enemiesForGroup[(int)spawners[i].generationGroup], spawners[i].transform.position, Quaternion.identity, content);
            }
            yield return null;
        }


        StartCoroutine(WaitBeforeCheck());
    }


    IEnumerator WaitBeforeCheck(){
        Messenger.Broadcast(GameEvent.ROOM_GENERATED);

        yield return new WaitForSeconds(.05f);

        FindEnemies();
        if(Enemies.Length>0) isRoomActive = true;

        List<string> requiredMeshes = new();
        foreach (Enemy enemy in Enemies)
        {
            requiredMeshes.Add(enemy.requiredNavMesh.ToString());
        }

        yield return null;

        foreach (NavMeshSurface nav in navMeshes)
        {
            if (!requiredMeshes.Contains(nav.name)) nav.gameObject.SetActive(false);
            // if (!requiredMeshes.Contains(nav.name)) Destroy(nav.gameObject);
        }

        yield return null;

        if(isStartingRoom) Destroy(reward);

        else if(isBossRoom) SetReward(rewardPrefab[Random.Range(0, rewardPrefab.Length)]);

        else if(isRoomActive){
            if(Random.Range(0+PlayerManager.Instance.LuckLevelled*5, 100)>50) SetReward(rewardPrefab[Random.Range(0, rewardPrefab.Length)]);
        }

        yield return null;

        foreach (DoorController door in doors)
        {
            if(isRoomActive)    door.CloseDoor();
            else                door.OpenDoor();
        }
    }

    public void SetReward(GameObject reward){
        this.reward = Instantiate(reward, content.GetChild(0).Find("Reward"));;
    }

    public void GenerateExit(){
        foreach (DoorController door in doors)
        {
            if(!door.gameObject.activeSelf){
                door.doorType = DoorType.NEXT_LEVEL;
                door.gameObject.SetActive(true);
                break;
            }
        }
    }



    
    // TODO: Da rimuovere in produzione
    // permette di eliminare tutti i nemici nella stanza con la barra spaziatrice, utile per testare cose
    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            Collider2D[] hit = Physics2D.OverlapBoxAll(roomCenter, new Vector2(roomX, roomY), 0);
            
            foreach (Collider2D player in hit)
            {
                if(player.CompareTag("Player")){
                    foreach (Collider2D item in hit)
                    {
                        if(item.CompareTag("Enemy")){
                            if(item.name.Contains("Jerry")){
                                item.GetComponent<Enemy>().TakeDamage(5);
                                return;
                            }
                            else item.GetComponent<Enemy>().TakeDamage(100);
                        }
                    }

                    break;
                }
            }
        }
    }




    public void ObstacleDestroyed(){
        foreach (NavMeshSurface nav in navMeshes)
        {
            if(nav != null && nav.gameObject != null && nav.gameObject.activeSelf) nav.UpdateNavMesh(nav.navMeshData);
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
        Collider2D[] tmp = Physics2D.OverlapBoxAll(roomCenter, new Vector2(roomX, roomY), 0, LayerMask.GetMask("Enemy"));

        Enemies = new Enemy[tmp.Length];
        for (int i = 0; i < tmp.Length; i++)
        {
            Enemies[i] = tmp[i].GetComponent<Enemy>();
        }
    }

    void CheckRoom(){
        FindEnemies();
        if(Enemies.Length==0) RoomCleared();
    }

    // Istanzia il premio e apre le porte della stanza
    void RoomCleared(){
        isRoomActive = false;
        if(reward) reward.SetActive(true);
        foreach (DoorController door in doors)
        {
            door.OpenDoor();
        }

        foreach (NavMeshSurface nav in navMeshes)
        {
            Destroy(nav.gameObject);
        }
    }

    // Quando il giocatore entra nella stanza, si imposta come stanza corrente e si attivano i nemici
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")){
            other.GetComponent<PlayerController>().SetCurrentRoom(this);
            PlayerManager.Instance.currentRoom = this;

            FindEnemies();

            if(reward && Enemies.Length>0) reward.SetActive(false);

            foreach (Enemy enemy in Enemies)
            {
                enemy.isActive = true;
            }

            foreach (Minion minion in PlayerManager.Instance.GetMinions().GetComponentsInChildren<Minion>())
            {
                if(minion.RequiredNavMesh != RequiredNavMesh.NONE){
                    foreach (NavMeshSurface nav in navMeshes)
                    {
                        if(nav.name.Equals(minion.RequiredNavMesh.ToString())){
                            nav.gameObject.SetActive(true);
                            break;
                        }
                    }
                }
            }
        }
    }

    // Quando il giocatore esce dalla stanza, si disattivano i nemici
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player")){
            foreach (Enemy enemy in Enemies)
            {
                enemy.isActive = false;
            }
        }
    }

    private void OnDestroy() {
        Messenger.RemoveListener(GameEvent.LEVEL_GENERATED, OnRoomGenerated);
        Messenger.RemoveListener(GameEvent.ROOM_GENERATED, OnRoomGenerated);

        StopAllCoroutines();
    }
}
