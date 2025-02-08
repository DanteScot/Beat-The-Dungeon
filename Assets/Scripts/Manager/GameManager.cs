using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

// Il GameManager è il manager che si occupa di gestire il gioco in generale, come il cambio di scena, la gestione degli eventi e la gestione degli audio monouso
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private AudioSource audioSource;
    [SerializeField] private GameObject canvas;
    private CanvasGroup canvasGroup;

    private readonly string basePath = "Sounds/";

    [SerializeField] private int level; // -2 menu, -1 tutorial, 0 lobby, n level n
    public int seed = 0;

    // Variabili per evitare che un suono venga riprodotto più volte nello stesso frame
    bool playerHit = false, enemyHit = false, itemPicked = false, gearPicked = false, bulletShoot = false;

    private void Awake() {
        if(Instance == null){
            Instance = this;

            audioSource = GetComponent<AudioSource>();

            if(canvas == null) canvas = GameObject.Find("Canvas");
            canvasGroup = canvas.GetComponent<CanvasGroup>();

            Messenger.AddListener(GameEvent.PLAYER_HIT, OnPlayerHit);
            Messenger.AddListener(GameEvent.ENEMY_HIT, OnEnemyHit);
            Messenger<string>.AddListener(GameEvent.ITEM_PICKED, OnItemPicked);
            Messenger.AddListener(GameEvent.GEAR_PICKED, OnGearPicked);
            Messenger.AddListener(GameEvent.BULLET_SHOOT, OnBulletShoot);
        } else {
            Destroy(gameObject);
        }
    }

    void OnDestroy() {
        Messenger.RemoveListener(GameEvent.PLAYER_HIT, OnPlayerHit);
        Messenger.RemoveListener(GameEvent.ENEMY_HIT, OnEnemyHit);
        Messenger<string>.RemoveListener(GameEvent.ITEM_PICKED, OnItemPicked);
        Messenger.RemoveListener(GameEvent.GEAR_PICKED, OnGearPicked);
        Messenger.RemoveListener(GameEvent.BULLET_SHOOT, OnBulletShoot);
    }

    private void Start() {
        if(canvasGroup != null){
            canvasGroup.alpha = 0;
            canvas.SetActive(false);
        }
        if(!(SceneManager.GetActiveScene().name.Equals("MainMenu") || SceneManager.GetActiveScene().name.Equals("Lobby"))){

            // TODO: decommentare
            // Instantiate(Resources.Load("Prefabs/808"), Vector3.zero, Quaternion.identity, PlayerManager.Instance.GetMinions());
        }
    }

    // Funzione generica per caricare una scena, con un effetto di fade in e fade out
    public async void LoadScene(string scene){
        GameEvent.canMove = false;
        Time.timeScale = 0;

        try{
            Destroy(Controller808.Instance.gameObject);
        } catch (System.Exception){}

        canvas.SetActive(true);
        
        while(canvasGroup.alpha < 1){
            canvasGroup.alpha += .01f;
            await Task.Delay(5);
        }

        SceneManager.LoadScene(scene);

        // Aspetta che la scena sia stata caricata
        while(!SceneManager.GetActiveScene().name.Equals(scene)){
            await Task.Delay(100);
        }

        // Se la scena non è il menu principale o la lobby, crea un 808
        if(!(scene.Equals("MainMenu") || scene.Equals("Lobby"))){
            Instantiate(Resources.Load("Prefabs/808"), Vector3.zero, Quaternion.identity, PlayerManager.Instance.GetMinions());
        }

        while(canvasGroup.alpha > 0){
            canvasGroup.alpha -= .01f;
            await Task.Delay(5);
        }

        canvas.SetActive(false);

        Time.timeScale = 1;
        GameEvent.canMove = true;
    }

    public int GetLevel(){
        return level;
    }
    
    public void LoadNextLevel(){
        level++;
        LoadScene("Level");
    }

    public void LoadLobby(){
        level = 0;
        LoadScene("Lobby");
    }

    public void LoadTutorial(){
        level = -1;
        LoadScene("Tutorial");
    }

    public void LoadMainMenu(){
        level = -2;
        LoadScene("MainMenu");
    }

    // Resetta le variabili per evitare che un suono venga riprodotto più volte nello stesso frame
    private void FixedUpdate() {
        playerHit = false;
        enemyHit = false;
        itemPicked = false;
        gearPicked = false;
        bulletShoot = false;
    }


    // Funzioni per gestire gli eventi e riprodurre i suoni corrispondenti
    // Alcuni suoni hanno un pitch leggermente diverso per evitare che suonino troppo ripetitivi
    // Il bullet oltre al pitch ha anche 4 suoni diversi per evitare che suoni troppo ripetitivo

    void OnPlayerHit(){
        if(playerHit) return;
        playerHit = true;
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(Resources.Load(basePath+"Player Hit") as AudioClip);
    }

    void OnEnemyHit(){
        if(enemyHit) return;
        enemyHit = true;
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(Resources.Load(basePath+"Enemy Hit") as AudioClip);
    }

    void OnItemPicked(string tmp){
        if(itemPicked) return;
        itemPicked = true;
        audioSource.pitch = 1;
        audioSource.PlayOneShot(Resources.Load(basePath+"Powerup Sound") as AudioClip);
    }

    void OnGearPicked(){
        if(gearPicked) return;
        gearPicked = true;
        audioSource.pitch = 1;
        audioSource.PlayOneShot(Resources.Load(basePath+"Gear Picked") as AudioClip);
    }

    void OnBulletShoot(){
        if(bulletShoot) return;
        bulletShoot = true;
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(Resources.Load(basePath+"Bullet "+Random.Range(1, 4)) as AudioClip);
    }
}
