using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Classe astratta per gli oggetti che si attivano a ritmo di musica

// Enum per settare l'intervallo di tempo tra un beat e l'altro
// Usato per far si che l'oggetto si attivi ogni 1/4, 1/2 o 1 beat
public enum intervalIndex{
    EveryQuarterBeat,
    EveryHalfBeat,
    EveryBeat
}

// Enum per settare su quale beat l'oggetto deve attivarsi
// Usato per far si che oggetti con lo stesso intervallo si attivino su beat diversi
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

    // Aumenta eventualmente il tempo di attesa all'inizio
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

        // Aggiunge il metodo OnBeat alla lista di metodi da chiamare ad ogni beat
        BeatManager.Instance.GetInterval().AddListener(OnBeat);

        // BeatManager.Instance.GetInterval(0).AddListener(OnBeat); // versione originale che però lasvciava meno libertà di utilizzo
    }

    // Setta il tempo di attesa tra un beat e l'altro
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

    // Metodo che verrà chiamato ad ogni beat
    // Si assicura che il metodo Trigger venga chiamato solo quando il tempo di attesa è terminato
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

    // Metodo che verrà chiamato quando l'oggetto deve attivarsi
    public abstract void Trigger();

    public void OnDestroy()
    {
        BeatManager.Instance.GetInterval().RemoveListener(Trigger);
        // BeatManager.Instance.GetInterval((int)index).RemoveListener(Trigger);
    }
}
