using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class RythmedObject : MonoBehaviour
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

    public abstract void Trigger();

    public void OnDestroy()
    {
        BeatManager.Instance.GetInterval((int)index).RemoveListener(Trigger);
    }
}
