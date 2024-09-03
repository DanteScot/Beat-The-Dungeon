using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum intervalIndex{
    EveryQuarterBeat,
    EveryHalfBeat,
    EveryBeat
}
public enum beatIndex{
    FirstBeat,
    SecondBeat,
    ThirdBeat,
    FourthBeat
}
public abstract class RythmedObject : MonoBehaviour
{
    [SerializeField] protected intervalIndex index;
    [SerializeField] protected beatIndex waitBeat;
    protected int beatToWait;

    public void Awake()
    {
        ResetCounter();

        switch (waitBeat)
        {
            case beatIndex.FirstBeat:
                beatToWait += 0;
                break;
            case beatIndex.SecondBeat:
                beatToWait += 1;
                break;
            case beatIndex.ThirdBeat:
                beatToWait += 2;
                break;
            case beatIndex.FourthBeat:
                beatToWait += 3;
                break;
        }

        BeatManager.Instance.GetInterval().AddListener(OnBeat);
        // BeatManager.Instance.GetInterval(0).AddListener(OnBeat);
    }

    public void ResetCounter()
    {
        switch(index){
            case intervalIndex.EveryQuarterBeat:
                beatToWait = 0;
                break;
            case intervalIndex.EveryHalfBeat:
                beatToWait = 1;
                break;
            case intervalIndex.EveryBeat:
                beatToWait = 3;
                break;
        }
    }

    protected void OnBeat()
    {
        if(beatToWait>0)
        {
            beatToWait--;
            return;
        }

        Trigger();
        
        ResetCounter();
    }

    public abstract void Trigger();

    public void OnDestroy()
    {
        BeatManager.Instance.GetInterval().RemoveListener(Trigger);
        // BeatManager.Instance.GetInterval((int)index).RemoveListener(Trigger);
    }
}
