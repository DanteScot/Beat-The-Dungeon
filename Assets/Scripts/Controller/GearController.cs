using UnityEngine;

// Enumeratore per la selezione del tipo di ingranaggio
public enum GearSelected
{
    random,
    lowGear,
    middleGear,
    highGear
}

// Classe per il controllo degli ingranaggi
public class GearController : PickUp
{
    public GearSelected gearSelected;

    [SerializeField] private Sprite[] gears;
    private int value;

    // Se è stato selezionato random, seleziona un ingranaggio in base alla probabilità
    // altrimenti seleziona l'ingranaggio scelto
    public new void Start()
    {
        base.Start();

        if (gearSelected == GearSelected.random)
        {
            gearSelected = GetWeightedRandomGear();
        }

        switch (gearSelected)
        {
            case GearSelected.lowGear:
                GetComponent<SpriteRenderer>().sprite = gears[0];
                value = 1;
                break;
            case GearSelected.middleGear:
                GetComponent<SpriteRenderer>().sprite = gears[1];
                value = 5;
                break;
            case GearSelected.highGear:
                GetComponent<SpriteRenderer>().sprite = gears[2];
                value = 10;
                break;
        }
    }


    // Funzione per calcolare la probabilità di selezionare un ingranaggio
    private float Sigmoid(float x, float k, float x0)
    {
        return 1 / (1 + Mathf.Exp(-k * (x - x0)));
    }

    private float LowGearProb(int x)
    {
        // float sigmoidC = Sigmoid(x, 1, 5);
        float sigmoidB = Sigmoid(x, 1, 3);
        return 1 - sigmoidB;
    }

    private float MiddleGearProb(int x)
    {
        float sigmoidB = Sigmoid(x, 1, 3);
        float sigmoidC = Sigmoid(x, 1, 5);
        return sigmoidB - sigmoidC;
    }

    // Non sembrerebbe servire
    // private float HighGearProb(int x)
    // {
    //     return Sigmoid(x, 1, 5);
    // }

    private GearSelected GetWeightedRandomGear()
    {
        int luck=(int)PlayerManager.Instance.LuckLevelled;
        float rand = Random.value;

        float probA = LowGearProb(luck);
        float probB = MiddleGearProb(luck);
        // float probC = HighGearProb(luck);

        // Debug.Log("Probabilities: " + probA + " " + probB + " " + probC);

        if (rand < probA)
        {
            return GearSelected.lowGear;
        }
        else if (rand < probA + probB)
        {
            return GearSelected.middleGear;
        }
        else
        {
            return GearSelected.highGear;
        }
    }

    public override void PickUpItem()
    {
        Messenger.Broadcast(GameEvent.GEAR_PICKED);
        PlayerManager.Instance.Gears += value;
        Destroy(gameObject);
    }
}