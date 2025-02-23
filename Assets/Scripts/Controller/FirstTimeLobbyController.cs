using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Classe che gestisce il comportamento della lobby la prima volta che il player ci entra
public class FirstTimeLobbyController : Interactable
{
    [SerializeField] private GameObject lightFather;
    [SerializeField] private GameObject[] lights;
    [SerializeField] private AudioClip powerDownSound;
    [SerializeField] private AudioClip powerUpSound;

    public override void Interact()
    {
        if(!isInteracting)
        {
            isInteracting = true;
            enabled = false;
            HideInteractMessage();
            StartCoroutine(InteractCoroutine());
        }
    }

    private IEnumerator InteractCoroutine()
    {
        yield return StartCoroutine(TurnOffLights());
        PlayerManager.Instance.LoadPlayerStats(new GameData());
        SaveSystem.SaveGame(new GameData());
    }

    // Spegne le luci "di emergenza" della lobby e riaccende le luci normali
    IEnumerator TurnOffLights()
    {
        GameEvent.canMove = false;
        int count=3;
        float waitTime = 0.15f;
        yield return new WaitForSeconds(1f);

        foreach (GameObject light in lights)
        {
            // Le prime count luci si spengono più lentamente
            float nextWaitTime = waitTime;
            if(count>0) nextWaitTime = waitTime*count*2;

            // Spegne la luce e fa partire il suono, dopodiché aspetta nextWaitTime secondi
            light.GetComponent<Light2D>().enabled = false;
            light.GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(nextWaitTime);

            count--;
        }

        // Fa la stessa cosa per la luce principale
        yield return new WaitForSeconds(.85f);
        Light2D incubatorLight = transform.parent.GetComponentInChildren<Light2D>();
        incubatorLight.intensity = 0;
        transform.parent.GetComponent<AudioSource>().PlayOneShot(powerDownSound);

        // Aspetta 2 secondi e poi riaccende la luce con il nuovo colore ed emette un suono
        incubatorLight.color = Color.white;
        yield return new WaitForSeconds(2f);

        transform.parent.GetComponent<AudioSource>().volume = 0.5f;
        transform.parent.GetComponent<AudioSource>().PlayOneShot(powerUpSound);

        // La luce si riaccende gradualmente
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime*0.25f;
            incubatorLight.intensity = Mathf.Lerp(0, 1.1f, t);
            incubatorLight.pointLightOuterRadius = Mathf.Lerp(0, 100, t);  
            incubatorLight.shadowIntensity = Mathf.Lerp(0.75f, 0, t);
            LobbyManager.Instance.ui.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(-1, 1, t);
            yield return null;
        }

        // Aspetta 2 secondi e poi fa partire l'animazione di 808 nella lobby
        // Sarà lui a gestire il resto
        yield return new WaitForSeconds(2f);
        Lobby808Controller.Instance.StartAnimation();

        // Disattiva la luce dell'incubatrice e attiva il controller dell'incubatrice
        // (Spegnere tutte le luci fa ritornare l'illuminazione al 2D base, ci servono più le luci dinamiche)
        incubatorLight.enabled = false;
        transform.parent.GetComponent<IncubatorController>().enabled = true;
        BeatManager.Instance.gameObject.SetActive(true);
        BeatManager.Instance.gameObject.GetComponent<AudioSource>().Play();

        Messenger.Broadcast(GameEvent.ACTIVATE_LOBBY);

        Destroy(gameObject);
    }
}
