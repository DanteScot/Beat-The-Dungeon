using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public enum PassiveItemSelected{
    random,
    isaac,
    FireFlower,
}

public class PassiveItem : PickUp
{
    public PassiveItemSelected passiveItemSelected;


    private string basePath = "";
    private string selected;
    private MethodInfo method;


    public void Start()
    {
        if(passiveItemSelected == PassiveItemSelected.random)
        {
            System.Array values = System.Enum.GetValues(typeof(PassiveItemSelected));
            passiveItemSelected = (PassiveItemSelected)values.GetValue(Random.Range(1, values.Length));
        }


        switch(passiveItemSelected)
        {
            case PassiveItemSelected.isaac:
                selected = "Isaac";
                break;
                
            case PassiveItemSelected.FireFlower:
                selected = "FireFlower";
                break;
        }


        SetSprite();
        SetMethod();
    }

    public override void PickUpItem()
    {
        method.Invoke(this, null);
    }

    private void SetSprite()
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(basePath + selected);
    }

    private void SetMethod()
    {
        method = GetType().GetMethod(selected, BindingFlags.NonPublic | BindingFlags.Instance);
    }


    // I SEGUENTI METODI VERRANNO CHIAMATI SOLO TRAMITE REFLECTION
    private void Isaac()
    {
        Debug.Log("Isaac");
    }

    private void FireFlower()
    {
        Debug.Log("FireFlower");
    }
}
