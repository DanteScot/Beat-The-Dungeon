using UnityEngine;
public enum IntervalIndex{
    EveryBeat,
    Every2Beat,
    Every4Beat
}
public enum WaitBeforeStart{
    FirstBeat,
    SecondBeat,
    ThirdBeat,
    FourthBeat
}

public abstract class RythmedObject : MonoBehaviour
{
    [SerializeField] protected IntervalIndex index;
    [SerializeField] protected WaitBeforeStart waitBeat;
    protected int beatToWait;

    public void Awake() {
        ResetCounter();

        switch (waitBeat)
        {
            case WaitBeforeStart.FirstBeat:
                beatToWait += 0;
                break;
            case WaitBeforeStart.SecondBeat:
                beatToWait += 1;
                break;
            case WaitBeforeStart.ThirdBeat:
                beatToWait += 2;
                break;
            case WaitBeforeStart.FourthBeat:
                beatToWait += 3;
                break;
        }

        BeatManager.Instance.AddListener(OnBeat);
    }

    public void ResetCounter() {
        switch(index){
            case IntervalIndex.EveryBeat:
                beatToWait = 0;
                break;
            case IntervalIndex.Every2Beat:
                beatToWait = 1;
                break;
            case IntervalIndex.Every4Beat:
                beatToWait = 3;
                break;
        }
    }

    protected void OnBeat() {
        if(beatToWait>0)
        {
            beatToWait--;
            return;
        }

        Trigger();
        
        ResetCounter();
    }

    public abstract void Trigger();

    public void OnDestroy() {
        BeatManager.Instance.RemoveListener(Trigger);
    }
}
