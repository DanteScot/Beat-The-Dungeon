using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FirstTimeLobbyController : Interactable
{
    [SerializeField] private GameObject lightFather;
    [SerializeField] private GameObject[] lights;
    // [SerializeField] private GameObject door;
    [SerializeField] private AudioClip powerDownSound;
    [SerializeField] private AudioClip powerUpSound;

    // public void  OnValidate()
    // {
    //     if(lightFather!=null)
    //     {
    //         lights = new GameObject[lightFather.transform.childCount];
    //         for (int i = 0; i < lightFather.transform.childCount; i++)
    //         {
    //             lights[i] = lightFather.transform.GetChild(i).gameObject;
    //         }
    //     }
    // }

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

    IEnumerator TurnOffLights()
    {
        GameEvent.canMove = false;
        int count=3;
        float waitTime = 0.15f;
        yield return new WaitForSeconds(1f);
        foreach (var light in lights)
        {
            var nextWaitTime = waitTime;
            if(count>0) nextWaitTime = waitTime*count*2;

            light.GetComponent<Light2D>().enabled = false;
            light.GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(nextWaitTime);

            count--;
        }
        yield return new WaitForSeconds(.85f);
        var incubatorLight = transform.parent.GetComponentInChildren<Light2D>();
        incubatorLight.intensity = 0;

        transform.parent.GetComponent<AudioSource>().PlayOneShot(powerDownSound);

        incubatorLight.color = Color.white;
        yield return new WaitForSeconds(2f);

        transform.parent.GetComponent<AudioSource>().volume = 0.5f;
        transform.parent.GetComponent<AudioSource>().PlayOneShot(powerUpSound);

        // interpolating light intensity
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

        yield return new WaitForSeconds(2f);
        Lobby808Controller.Instance.StartAnimation();

        incubatorLight.enabled = false;
        transform.parent.GetComponent<IncubatorController>().enabled = true;
        BeatManager.Instance.gameObject.SetActive(true);
        BeatManager.Instance.gameObject.GetComponent<AudioSource>().Play();


        Destroy(gameObject);
    }
}
