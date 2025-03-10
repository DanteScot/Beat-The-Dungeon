using UnityEngine;
using UnityEngine.Rendering.Universal;

// Classe responsabile della gestione della lobby
public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public CanvasGroup ui;

    [SerializeField] private AudioClip lobbyMusic;
    [SerializeField] private int bpm; 

    private bool isFirstTime;

    // Elementi da distruggere se non è la prima volta che si gioca
    [SerializeField] private GameObject[] itemToDestroy;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ui = transform.parent.Find("Canvas").GetComponent<CanvasGroup>();
            ui.alpha = 0;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Se non sono presenti dati salvati, allora è la prima volta che si gioca
        GameEvent.isInLobby = true;
        GameData data = SaveSystem.LoadGame();
        if (data != null) isFirstTime = false;
        else isFirstTime = true;

        if (!isFirstTime){
            NotFirstTimeLobby();
            PlayerManager.Instance.LoadPlayerStats(data);
        }
        else{
            Messenger.Broadcast(GameEvent.DEACTIVATE_LOBBY);
            BeatManager.Instance.gameObject.SetActive(false);
        }
    }

    // Imposta la lobby per evitare tutte le cose che avverrebbero al primo avvio del gioco
    void NotFirstTimeLobby(){
        BeatManager.Instance.AudioSource.Stop();
        BeatManager.Instance.BPM = bpm;
        BeatManager.Instance.AudioSource.clip = lobbyMusic;
        BeatManager.Instance.AudioSource.Play();

        Messenger.Broadcast(GameEvent.ACTIVATE_LOBBY);

        foreach (GameObject item in itemToDestroy)
        {
            Destroy(item);
        }

        foreach (Light2D item in FindObjectsOfType<Light2D>())
        {
            item.enabled = false;
        }

        foreach (Interactable item in FindObjectsOfType<Interactable>())
        {
            item.enabled = true;
        }

        Lobby808Controller.Instance.NoAnimation();

        GameEvent.canMove = true;
        GameEvent.ifFirstTime = false;

        ui.alpha=1;
    }
}
