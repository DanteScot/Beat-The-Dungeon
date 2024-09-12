using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private AudioSource audioSource;
    private CanvasGroup canvasGroup;

    private string basePath = "Sounds/";

    private int level = 0; // -1 tutorial, 0 lobby, n level n

    private void Awake() {
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            canvasGroup = GetComponentInChildren<CanvasGroup>();

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
        canvasGroup.alpha = 0;
        canvasGroup.gameObject.SetActive(false);
    }

    public async void LoadScene(string scene){
        GameEvent.canMove = false;
        Time.timeScale = 0;

        canvasGroup.gameObject.SetActive(true);
        
        while(canvasGroup.alpha < 1){
            canvasGroup.alpha += .01f;
            await Task.Delay(5);
        }

        SceneManager.LoadScene(scene);

        while(!SceneManager.GetActiveScene().name.Equals(scene)){
            await Task.Delay(100);
        }

        if(!(scene.Equals("MainMenu") || scene.Equals("Lobby"))){
            Instantiate(Resources.Load("Prefabs/808"));
        }

        while(canvasGroup.alpha > 0){
            canvasGroup.alpha -= .01f;
            await Task.Delay(5);
        }

        canvasGroup.gameObject.SetActive(false);

        Time.timeScale = 1;
        GameEvent.canMove = true;
    }

    public int GetLevel(){
        return level;
    }
    
    public void LoadNextLevel(){
        level++;
        LoadScene("Level "+level);
    }

    public void LoadTutorial(){
        level = -1;
        LoadScene("Tutorial");
    }

    public void LoadLobby(){
        level = 0;
        LoadScene("Lobby");
    }

    public void LoadMainMenu(){
        level = -2;
        LoadScene("MainMenu");
    }


    void OnPlayerHit(){
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(Resources.Load(basePath+"Player Hit") as AudioClip);
    }

    void OnEnemyHit(){
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(Resources.Load(basePath+"Enemy Hit") as AudioClip);
    }

    void OnItemPicked(string tmp){
        audioSource.pitch = 1;
        audioSource.PlayOneShot(Resources.Load(basePath+"Powerup Sound") as AudioClip);
    }

    void OnGearPicked(){
        audioSource.pitch = 1;
        audioSource.PlayOneShot(Resources.Load(basePath+"Gear Picked") as AudioClip);
    }

    void OnBulletShoot(){
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(Resources.Load(basePath+"Bullet "+Random.Range(1, 4)) as AudioClip);
    }
}
