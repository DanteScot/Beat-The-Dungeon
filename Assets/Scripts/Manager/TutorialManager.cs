using System.Collections;
using System.Collections.Generic;
using HeneGames.DialogueSystem;
using UnityEngine;

// Manager che si occupa di gestire il tutorial
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    // Lista di frasi che il tutorial deve dire
    [SerializeField] private List<NPC_CentenceList> tutorialSentences = new();

    private DialogueManager dialogueManager;
    private CircleCollider2D circleCollider;

    public bool stepCompleted = false;
    public bool started = false;

    private int currentSentenceIndex = -1;

    private void Awake() {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        dialogueManager = GetComponent<DialogueManager>();
        circleCollider = GetComponent<CircleCollider2D>();
        GameEvent.isInLobby = false;
    }

    // Viene chiamato alla fine del dialogo introduttivo
    public void Init(){
        if(!gameObject.activeSelf) return;

        // ho provato a fare RemoveAllListeners ma non funziona
        dialogueManager.endDialogueEvent=new UnityEngine.Events.UnityEvent();

        StartCoroutine(StartTutorial());
        started = true;
        stepCompleted = true;
    }
    
    // Passa alla frase successiva, disabilita e riabilita il collider per attivare l'onTriggerEnter2D
    public void NextSentence(){
        currentSentenceIndex++;
        if(currentSentenceIndex < tutorialSentences.Count){
            dialogueManager.sentences = tutorialSentences[currentSentenceIndex].sentences;
            
            circleCollider.enabled = false;
            circleCollider.enabled = true;

            // Se è l'ultima frase aggiunge un listener per chiamare EndTutorial
            if(currentSentenceIndex == tutorialSentences.Count-1){
                dialogueManager.endDialogueEvent.AddListener(EndTutorial);
            }
        }
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.F6)) EndTutorial();

        if(!started) return;

        switch(currentSentenceIndex){
            case 0:
                // MOVEMENT TUTORIAL (Gestito da TutorialMovementController)
                break;
            case 1:
                // ATTACK TUTORIAL
                if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)){
                    // Se c'è un proiettile presente lo distrugge
                    if(FindObjectOfType<BulletController>() != null) Destroy(FindObjectOfType<BulletController>().gameObject);

                    stepCompleted = true;
                    started = false;
                }
                break;
            case 2:
                // RANGE ATTACK TUTORIAL
                // Controlla che sia stato sparato un proiettile
                if(FindObjectOfType<BulletController>() != null){
                    stepCompleted = true;
                    started = false;
                }
                break;
            
            default:
                break;
        }
    }

    // Viene Chiamato alla fine dei dialoghi per riprendere il tutorial
    public void StartCheck(){
        started = true;
    }

    // Coroutine che gestisce le fasi del tutorial
    private IEnumerator StartTutorial(){
        while(currentSentenceIndex < tutorialSentences.Count){
            yield return new WaitUntil(()=>stepCompleted);
            stepCompleted = false;
            NextSentence();
        }
    }

    // Chiude il tutorial e carica la lobby
    public void EndTutorial(){
        if(!gameObject.activeSelf) return;

        Destroy(Controller808.Instance.gameObject);
        GameManager.Instance.LoadLobby();
    }
}
