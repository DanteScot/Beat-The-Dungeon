using TMPro;
using UnityEngine;

// Classe astratta per gli oggetti interagibili (con interagibile si intende un oggetto con cui il giocatore può interagire premendo E)
[RequireComponent(typeof(CircleCollider2D))]
public class Interactable : MonoBehaviour
{
    // Messaggio che verrà mostrato al giocatore quando è vicino all'oggetto
    [SerializeField] private string interactMessage  = "interact";
    
    private GameObject interactMessageGameobject;
    private TextMeshProUGUI interactMessageText;

    private bool isPlayerInRange = false;
    protected bool isInteracting = false;

    private void OnValidate() {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    public void Start(){
        if(enabled){
            interactMessageGameobject=PlayerManager.Instance.GetPlayer().Find("UI").Find("Interactable").gameObject;
            interactMessageText=interactMessageGameobject.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    // Start e OnEnable fanno la stessa cosa, per essere sicuri che venga chiamato in tutti i casi possibili
    void OnEnable(){
        try{
            interactMessageGameobject=PlayerManager.Instance.GetPlayer().Find("UI").Find("Interactable").gameObject;
            interactMessageText=interactMessageGameobject.GetComponentInChildren<TextMeshProUGUI>();
        } catch (System.Exception){}
    }

    // Mostra il messaggio di interazione
    protected void ShowInteractMessage(){
        interactMessageText.text=interactMessage;
        isPlayerInRange = true;
        interactMessageGameobject.SetActive(true);
    }

    // Nasconde il messaggio di interazione
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

    // Si assicura che l'interazione avvenga solo se il giocatore è vicino all'oggetto e preme E
    // Per evitare che l'interazione venga chiamata più volte viene usata la variabile isInteracting
    void FixedUpdate()
    {
        if(isPlayerInRange && Input.GetKeyDown(KeyCode.E) && enabled  && !isInteracting)
        {
            Interact();
        }
    }

    // Metodo che verrà implementato nelle classi figlie per definire il comportamento dell'oggetto interagibile
    public virtual void Interact(){
        Debug.Log("Interacting with "+gameObject.name);
    }
}
