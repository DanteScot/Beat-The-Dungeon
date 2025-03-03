using UnityEngine;

// Classe responsabile della gestione della pausa del gioco
public class PauseController : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    void Awake(){
        canvasGroup = GetComponent<CanvasGroup>();

        Messenger.AddListener(GameEvent.GAME_PAUSED, Pause);
        Messenger.AddListener(GameEvent.GAME_RESUMED, Resume);
    }

    void OnDestroy(){
        Messenger.RemoveListener(GameEvent.GAME_PAUSED, Pause);
        Messenger.RemoveListener(GameEvent.GAME_RESUMED, Resume);
    }

    // Attiva il menu di pausa
    void Pause(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;

        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        BeatManager.Instance.AudioSource.Pause();
    }

    // Disattiva il menu di pausa
    void Resume(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1;

        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        BeatManager.Instance.AudioSource.UnPause();
    }

    // Riprende il gioco
    public void OnResumeButton(){
        GameEvent.IsPaused = false;
    }

    // Torna alla lobby
    public void OnLobbyButton(){
        PlayerManager.Instance.EndGame();
        GameEvent.IsPaused = false;
        GameManager.Instance.LoadLobby();
    }

    // Ritora al menu principale
    public void OnQuitButton(){
        PlayerManager.Instance.EndGame();
        GameEvent.IsPaused = false;
        GameManager.Instance.LoadMainMenu();
    }
}
