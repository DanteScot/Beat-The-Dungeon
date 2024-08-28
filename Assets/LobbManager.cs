using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LobbManager : MonoBehaviour
{
    public static LobbManager Instance { get; private set; }

    public bool isFirstTime = true;

    [SerializeField] private GameObject[] itemToDestroy;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // TODO: Check if this is the first time the player is playing the game

        if (!isFirstTime){
            NotFirstTimeLobby();
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
    }
}
