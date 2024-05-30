using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearCountController : MonoBehaviour, Observer
{
    private int gearCount;

    public void Notify()
    {
        gearCount = PlayerManager.Instance.Gears;
        GetComponent<TMPro.TextMeshProUGUI>().text = gearCount.ToString();
    }

    void Start()
    {
        PlayerManager.Instance.Attach(this);
        Notify();
    }
    
    public void OnDestroy()
    {
        PlayerManager.Instance.Detach(this);
    }
}
