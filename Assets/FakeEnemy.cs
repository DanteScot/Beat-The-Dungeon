using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FakeEnemy : Interactable
{
    [SerializeField] private GameObject lightFather;
    [SerializeField] private GameObject[] lights;
    [SerializeField] private GameObject door;

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
        StartCoroutine(TurnOffLights());
    }

    IEnumerator TurnOffLights()
    {
        int count=3;
        float waitTime = 0.15f;
        yield return new WaitForSeconds(1f);
        foreach (var light in lights)
        {
            var nextWaitTime = waitTime;
            if(count>0) nextWaitTime = waitTime*count*2;

            light.GetComponent<Light2D>().enabled = false;
            yield return new WaitForSeconds(nextWaitTime);

            count--;
        }
        yield return new WaitForSeconds(.85f);
        var tmp = transform.parent.GetComponentInChildren<Light2D>();
        tmp.intensity = 0;
        tmp.color = Color.white;
        yield return new WaitForSeconds(2f);

        // interpolating light intensity
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime*0.25f;
            tmp.intensity = Mathf.Lerp(0, 1, t);
            tmp.pointLightOuterRadius = Mathf.Lerp(0, 100, t);
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        tmp.enabled = false;
    }
}
