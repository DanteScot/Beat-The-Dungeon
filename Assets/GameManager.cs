using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private CanvasGroup canvasGroup;

    private int level = 0; // -1 tutorial, 0 lobby, n level n
    private string currentScene;

    private void Awake() {
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);

            canvasGroup = GetComponentInChildren<CanvasGroup>();
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        canvasGroup.alpha = 0;
        canvasGroup.gameObject.SetActive(false);

        currentScene = SceneManager.GetActiveScene().name;
    }

    public async void LoadScene(string scene){
        currentScene = SceneManager.GetActiveScene().name;

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
        level = 0;
        LoadScene("MainMenu");
    }
}
