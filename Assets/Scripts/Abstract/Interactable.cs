using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Interactable : MonoBehaviour
{
    // public bool isInteractable = true;
    [SerializeField] private string interactMessage  = "interact";
    
    private GameObject interactMessageGameobject;
    private TextMeshProUGUI interactMessageText;

    private bool isPlayerInRange = false;
    protected bool isInteracting = false;

    void Start(){
        if(this.enabled){
            interactMessageGameobject=PlayerManager.Instance.GetPlayer().Find("UI").Find("Interactable").gameObject;
            interactMessageText=interactMessageGameobject.GetComponentInChildren<TextMeshProUGUI>();
            // interactMessageGameobject.SetActive(false);
        }
    }

    void OnEnable(){
        interactMessageGameobject=PlayerManager.Instance.GetPlayer().Find("UI").Find("Interactable").gameObject;
        interactMessageText=interactMessageGameobject.GetComponentInChildren<TextMeshProUGUI>();
        // interactMessageGameobject.SetActive(false);
    }

    protected void ShowInteractMessage(){
        interactMessageText.text=interactMessage;
        isPlayerInRange = true;
        interactMessageGameobject.SetActive(true);
    }

    protected void HideInteractMessage(){
        interactMessageGameobject.SetActive(false);
        isPlayerInRange = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && enabled)
        {
            ShowInteractMessage();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && enabled)
        {
            HideInteractMessage();
        }
    }

    void Update()
    {
        if(isPlayerInRange && Input.GetKeyDown(KeyCode.E) && enabled)
        {
            Interact();
        }
    }

    public virtual void Interact(){
        Debug.Log("Interacting with "+gameObject.name);
    }
}
