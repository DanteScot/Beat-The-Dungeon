using System.Collections;
using System.Collections.Generic;
using HeneGames.DialogueSystem;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [SerializeField] private List<NPC_CentenceList> tutorialSentences = new List<NPC_CentenceList>();

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

    public void Init(){
        if(!gameObject.activeSelf) return;

        // ho provato a fare RemoveAllListeners ma non funziona
        dialogueManager.endDialogueEvent=new UnityEngine.Events.UnityEvent();

        StartCoroutine(StartTutorial());
        started = true;
        stepCompleted = true;
    }
    
    public void NextSentence(){
        currentSentenceIndex++;
        if(currentSentenceIndex < tutorialSentences.Count){
            dialogueManager.sentences = tutorialSentences[currentSentenceIndex].sentences;
            
            circleCollider.enabled = false;
            circleCollider.enabled = true;
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
                    if(FindObjectOfType<BulletController>() != null) Destroy(FindObjectOfType<BulletController>().gameObject);

                    stepCompleted = true;
                    started = false;
                }
                break;
            case 2:
                // RANGE ATTACK TUTORIAL
                if(FindObjectOfType<BulletController>() != null){
                    stepCompleted = true;
                    started = false;
                }
                break;
            
            default:
                break;
        }
    }

    public void StartCheck(){
        started = true;
    }

    private IEnumerator StartTutorial(){
        while(currentSentenceIndex < tutorialSentences.Count){
            yield return new WaitUntil(()=>stepCompleted);
            stepCompleted = false;
            NextSentence();
        }

        Debug.Log("Tutorial completato");

    }

    public void EndTutorial(){
        StartCoroutine(Wait());
    }

    IEnumerator Wait(){
        yield return new WaitForSeconds(1);
        GameManager.Instance.LoadLobby();
    }
}
