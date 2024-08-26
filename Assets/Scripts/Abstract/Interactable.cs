using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Interactable : MonoBehaviour
{
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private string interactMessage  = "interact";
    
    private GameObject interactMessageGameobject;
    private TextMeshProUGUI interactMessageText;

    private bool isPlayerInRange = false;
    // private bool isInteracting = false;

    void Start(){
        interactMessageGameobject=PlayerManager.Instance.GetPlayer().Find("UI").Find("Interactable").gameObject;
        interactMessageText=interactMessageGameobject.GetComponentInChildren<TextMeshProUGUI>();
        interactMessageGameobject.SetActive(false);
    }

    void ShowInteractMessage(){
        interactMessageText.text=interactMessage;
        isPlayerInRange = true;
        interactMessageGameobject.SetActive(true);
    }

    void HideInteractMessage(){
        interactMessageGameobject.SetActive(false);
        isPlayerInRange = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && isInteractable)
        {
            ShowInteractMessage();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && isInteractable)
        {
            HideInteractMessage();
        }
    }

    void Update()
    {
        if(isPlayerInRange && isInteractable && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    public virtual void Interact(){
        Debug.Log("Interacting with "+gameObject.name);
    }
}
