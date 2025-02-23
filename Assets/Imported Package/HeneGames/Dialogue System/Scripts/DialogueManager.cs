using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HeneGames.DialogueSystem
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class DialogueManager : MonoBehaviour
    {
        private int currentSentence;
        private float coolDownTimer;
        private bool dialogueIsOn;
        private DialogueTrigger dialogueTrigger;

        private string lastCharacter;

        public enum TriggerState
        {
            Collision,
            Input
        }

        [Header("References")]
        [SerializeField] private AudioSource audioSource;

        [Header("Events")]
        public UnityEvent startDialogueEvent;
        public UnityEvent nextSentenceDialogueEvent;
        public UnityEvent endDialogueEvent;

        [Header("Dialogue")]
        [SerializeField] private TriggerState triggerState;
        public List<NPC_Centence> sentences = new();

        void OnValidate()
        {
            audioSource = GetComponent<AudioSource>();
            GetComponent<CircleCollider2D>().isTrigger = true;
        }

        private void Start()
        {
            audioSource.outputAudioMixerGroup = DialogueUI.instance.AudioMixerGroup;
        }

        private void Update()
        {
            //Timer
            if(coolDownTimer > 0f)
            {
                coolDownTimer -= Time.deltaTime;
            }

            //Start dialogue by input
            if (Input.GetKeyDown(DialogueUI.instance.actionInput) && dialogueTrigger != null && !dialogueIsOn)
            {
                //Trigger event inside DialogueTrigger component
                if (dialogueTrigger != null)
                {
                    dialogueTrigger.startDialogueEvent.Invoke();
                }

                startDialogueEvent.Invoke();

                //If component found start dialogue
                DialogueUI.instance.StartDialogue(this);

                //Hide interaction UI
                DialogueUI.instance.ShowInteractionUI(false);

                dialogueIsOn = true;
            }
        }

        //Start dialogue by trigger
        private void OnTriggerEnter(Collider other)
        {
            if (triggerState == TriggerState.Collision && !dialogueIsOn)
            {
                //Try to find the "DialogueTrigger" component in the crashing collider
                if (other.gameObject.TryGetComponent<DialogueTrigger>(out DialogueTrigger _trigger))
                {
                    //Trigger event inside DialogueTrigger component and store refenrece
                    dialogueTrigger = _trigger;
                    dialogueTrigger.startDialogueEvent.Invoke();

                    startDialogueEvent.Invoke();

                    //If component found start dialogue
                    DialogueUI.instance.StartDialogue(this);

                    dialogueIsOn = true;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (triggerState == TriggerState.Collision && !dialogueIsOn)
            {
                //Try to find the "DialogueTrigger" component in the crashing collider
                if (collision.gameObject.TryGetComponent<DialogueTrigger>(out DialogueTrigger _trigger))
                {
                    //Trigger event inside DialogueTrigger component and store refenrece
                    dialogueTrigger = _trigger;
                    dialogueTrigger.startDialogueEvent.Invoke();

                    startDialogueEvent.Invoke();

                    //If component found start dialogue
                    DialogueUI.instance.StartDialogue(this);

                    dialogueIsOn = true;
                }
            }
        }

        //Start dialogue by pressing DialogueUI action input
        private void OnTriggerStay(Collider other)
        {
            if (dialogueTrigger != null)
                return;

            if (triggerState == TriggerState.Input && dialogueTrigger == null)
            {
                //Try to find the "DialogueTrigger" component in the crashing collider
                if (other.gameObject.TryGetComponent<DialogueTrigger>(out DialogueTrigger _trigger))
                {
                    //Show interaction UI
                    DialogueUI.instance.ShowInteractionUI(true);

                    //Store refenrece
                    dialogueTrigger = _trigger;
                }
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (dialogueTrigger != null)
                return;

            if (triggerState == TriggerState.Input && dialogueTrigger == null)
            {
                //Try to find the "DialogueTrigger" component in the crashing collider
                if (collision.gameObject.TryGetComponent<DialogueTrigger>(out DialogueTrigger _trigger))
                {
                    //Show interaction UI
                    DialogueUI.instance.ShowInteractionUI(true);

                    //Store refenrece
                    dialogueTrigger = _trigger;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //Try to find the "DialogueTrigger" component from the exiting collider
            if (other.gameObject.TryGetComponent<DialogueTrigger>(out DialogueTrigger _trigger))
            {
                //Hide interaction UI
                DialogueUI.instance.ShowInteractionUI(false);

                //Stop dialogue
                StopDialogue();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            //Try to find the "DialogueTrigger" component from the exiting collider
            if (collision.gameObject.TryGetComponent<DialogueTrigger>(out DialogueTrigger _trigger))
            {
                //Hide interaction UI
                DialogueUI.instance.ShowInteractionUI(false);

                //Stop dialogue
                StopDialogue();
            }
        }

        public void StartDialogue()
        {
            //Start event
            if(dialogueTrigger != null)
            {
                dialogueTrigger.startDialogueEvent.Invoke();
            }

            //Reset sentence index
            currentSentence = 0;

            //Show first sentence in dialogue UI
            ShowCurrentSentence();

            //Play dialogue sound
            PlaySound(sentences[currentSentence].sentenceSound);

            //Cooldown timer
            coolDownTimer = sentences[currentSentence].skipDelayTime;
        }

        public void NextSentence(out bool lastSentence)
        {
            //The next sentence cannot be changed immediately after starting
            if (coolDownTimer > 0f)
            {
                lastSentence = false;
                return;
            }

            //Add one to sentence index
            currentSentence++;

            //Next sentence event
            if (dialogueTrigger != null)
            {
                dialogueTrigger.nextSentenceDialogueEvent.Invoke();
            }

            nextSentenceDialogueEvent.Invoke();

            //If last sentence stop dialogue and return
            if (currentSentence > sentences.Count - 1)
            {
                StopDialogue();

                lastSentence = true;

                return;
            }

            //If not last sentence continue...
            lastSentence = false;

            //Play dialogue sound
            PlaySound(sentences[currentSentence].sentenceSound);

            //Show next sentence in dialogue UI
            ShowCurrentSentence();

            //Cooldown timer
            coolDownTimer = sentences[currentSentence].skipDelayTime;
        }

        public void StopDialogue()
        {
            //Stop dialogue event
            if (dialogueTrigger != null)
            {
                dialogueTrigger.endDialogueEvent.Invoke();
            }

            endDialogueEvent.Invoke();

            //Hide dialogue UI
            DialogueUI.instance.ClearText();

            //Stop audiosource so that the speaker's voice does not play in the background
            if(audioSource != null)
            {
                audioSource.Stop();
            }

            //Remove trigger refence
            dialogueIsOn = false;
            dialogueTrigger = null;

            TalkingCharacter("");
        }

        private void PlaySound(AudioClip _audioClip)
        {
            //Play the sound only if it exists
            if (_audioClip == null || audioSource == null)
                return;

            //Stop the audioSource so that the new sentence does not overlap with the old one
            audioSource.Stop();

            //Play sentence sound
            audioSource.PlayOneShot(_audioClip);
        }

        private void ShowCurrentSentence()
        {
            if (sentences[currentSentence].dialogueCharacter != null)
            {
                TalkingCharacter(sentences[currentSentence].dialogueCharacter.name);

                //Show sentence on the screen
                DialogueUI.instance.ShowSentence(sentences[currentSentence].dialogueCharacter, sentences[currentSentence].sentence);

                //Invoke sentence event
                sentences[currentSentence].sentenceEvent.Invoke();
            }
            else
            {
                TalkingCharacter("");

                DialogueCharacter _dialogueCharacter = new();
                _dialogueCharacter.characterName = "";
                _dialogueCharacter.characterPhoto = null;

                DialogueUI.instance.ShowSentence(_dialogueCharacter, sentences[currentSentence].sentence);

                //Invoke sentence event
                sentences[currentSentence].sentenceEvent.Invoke();
            }
        }

        public int CurrentSentenceLenght()
        {
            if(sentences.Count <= 0)
                return 0;

            return sentences[currentSentence].sentence.Length;
        }





        //--------------------------------------------------------------------------------
        // Questa parte è stata aggiunta da me per comodità

        private void TalkingCharacter(string character){
            MakeTalk(lastCharacter, false);

            lastCharacter = character;

            MakeTalk(character, true);
        }

        private void MakeTalk(string character, bool talk){
            switch(character){
                case "808": 
                    if(GameEvent.isInLobby) Lobby808Controller.Instance.IsTalking = talk;
                    else Controller808.Instance.IsTalking = talk;
                    break;
                
                default:
                    break;
            }
        }
    }

    [System.Serializable]
    public class NPC_Centence
    {
        [Header("------------------------------------------------------------")]

        public DialogueCharacter dialogueCharacter;

        [TextArea(3, 10)]
        public string sentence;

        public float skipDelayTime = 0.5f;

        public AudioClip sentenceSound;

        public UnityEvent sentenceEvent;
    }
}