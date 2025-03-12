using TMPro;
using UnityEngine;

public class SeedInjectorController : Interactable
{
    [SerializeField] private Canvas seedInjectorUI;
    [SerializeField] private TMP_InputField text;
    [SerializeField] private IncubatorController incubatorController;

    private void Awake() {
        seedInjectorUI.enabled = false;

        Messenger.AddListener(GameEvent.ACTIVATE_LOBBY, Activate);
        Messenger.AddListener(GameEvent.DEACTIVATE_LOBBY, Deactivate);
    }

    void Activate(){
        enabled = true;
    }

    void Deactivate(){
        enabled = false;
    }

    public override void Interact()
    {
        isInteracting = true;
        GameEvent.canMove = false;
        seedInjectorUI.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;

        text.text = GameManager.Instance.seed.ToString();
        text.Select();
    }

    public void Confirm(){
        int value = 0;

        if(text.text != "") value = int.Parse(text.text);

        GameManager.Instance.seed = value;

        CloseUI();
    }

    public void CloseUI(){
        GameEvent.canMove = true;
        seedInjectorUI.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1;

        isInteracting = false;
    }

    private void OnDestroy() {
        Messenger.RemoveListener(GameEvent.ACTIVATE_LOBBY, Activate);
        Messenger.RemoveListener(GameEvent.DEACTIVATE_LOBBY, Deactivate);
    }
}
