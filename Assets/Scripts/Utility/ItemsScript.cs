using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;


public enum ItemSelected{
    random,
    isaac,
    FireFlower,
}

public class Items : PickUp
{
    public ItemSelected itemSelected;


    private string basePath = "";
    private string selected;
    private MethodInfo method;


    public new void Start()
    {
        base.Start();
        
        if(itemSelected == ItemSelected.random)
        {
            System.Array values = System.Enum.GetValues(typeof(ItemSelected));
            itemSelected = (ItemSelected)values.GetValue(Random.Range(1, values.Length));
        }


        switch(itemSelected)
        {
            case ItemSelected.isaac:
                selected = "Isaac";
                break;
                
            case ItemSelected.FireFlower:
                selected = "FireFlower";
                break;
        }


        SetSprite();
        SetMethod();
    }

    public void SetItem(ItemSelected ItemSelected)
    {
        this.itemSelected = ItemSelected;
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
