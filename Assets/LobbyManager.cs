using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public CanvasGroup ui;

    private bool isFirstTime;

    [SerializeField] private GameObject[] itemToDestroy;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ui = GameObject.Find("Canvas").GetComponent<CanvasGroup>();
            ui.alpha = 0;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameEvent.isInLobby = true;
        GameData data = SaveSystem.LoadGame();
        if (data != null) isFirstTime = false;
        else isFirstTime = true;

        if (!isFirstTime){
            NotFirstTimeLobby();
            PlayerManager.Instance.LoadPlayerStats(data);
        }
        else{
            BeatManager.Instance.gameObject.SetActive(false);
        }
    }

    void NotFirstTimeLobby(){
        foreach (var item in itemToDestroy)
        {
            Destroy(item);
        }

        foreach (var item in FindObjectsOfType<Light2D>())
        {
            item.enabled = false;
        }

        foreach (var item in FindObjectsOfType<Interactable>())
        {
            item.enabled = true;
        }

        Lobby808Controller.Instance.NoAnimation();

        GameEvent.canMove = true;
        GameEvent.ifFirstTime = false;

        ui.alpha=1;
    }
}
