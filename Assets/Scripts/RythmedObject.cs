using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RythmedObject : MonoBehaviour
{
    protected enum intervalIndex{
        EveryQuarterBeat,
        EveryHalfBeat,
        EveryBeat
    }
    [SerializeField] protected intervalIndex index;

    public void Start()
    {
        BeatManager.Instance.GetInterval((int)index).AddListener(Trigger);
    }

    public virtual void Trigger()
    {
        // Debug.Log("Triggered");
    }
}
