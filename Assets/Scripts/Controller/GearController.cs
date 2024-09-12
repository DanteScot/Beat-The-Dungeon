using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;


public enum GearSelected{
    random,
    lowGear,
    middleGear,
    highGear
}

public class GearController : PickUp
{
    public GearSelected gearSelected;

    [SerializeField] private Sprite[] gears;
    private int value;

    public new void Start()
    {
        base.Start();
        
        if(gearSelected == GearSelected.random)
        {
            System.Array availableGears = System.Enum.GetValues(typeof(GearSelected));
            gearSelected = (GearSelected)availableGears.GetValue(Random.Range(1, availableGears.Length));
        }


        switch(gearSelected)
        {
            case GearSelected.lowGear:
                GetComponent<SpriteRenderer>().sprite=gears[0];
                value=1;
                break;
            case GearSelected.middleGear:
                GetComponent<SpriteRenderer>().sprite=gears[1];
                value=5;
                break;
            case GearSelected.highGear:
                GetComponent<SpriteRenderer>().sprite=gears[2];
                value=10;
                break;
        }
    }

    public void SetGear(GearSelected gearSelected)
    {
        this.gearSelected = gearSelected;
    }

    public override void PickUpItem()
    {
        Messenger.Broadcast(GameEvent.GEAR_PICKED);
        PlayerManager.Instance.Gears += value;
        Destroy(gameObject);
    }
}
