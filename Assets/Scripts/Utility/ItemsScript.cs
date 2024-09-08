using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;


public enum ItemSelected{
    Random,
    Isaac,
    Jack,
    FireFlower,
}

public class Items : PickUp
{
    public ItemSelected itemSelected;


    private string basePath = "";
    private string selected;
    // private MethodInfo method;


    public new void Start()
    {
        base.Start();
        
        if(itemSelected == ItemSelected.Random)
        {
            Array values = Enum.GetValues(typeof(ItemSelected));
            itemSelected = (ItemSelected)values.GetValue(UnityEngine.Random.Range(1, values.Length));
        }

        selected = Enum.GetName(typeof(ItemSelected), itemSelected);

        SetSprite();
        // SetMethod();
    }

    public void SetItem(ItemSelected itemSelected)
    {
        this.itemSelected = itemSelected;
    }

    public override void PickUpItem()
    {
        // method.Invoke(this, null);
        PlayerManager.Instance.GetPlayer().GetComponent<PlayerController>().powers.Add(selected);
    }

    private void SetSprite()
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(basePath + selected);
    }

    // private void SetMethod()
    // {
    //     method = GetType().GetMethod(selected, BindingFlags.NonPublic | BindingFlags.Instance);
    // }


    // I SEGUENTI METODI VERRANNO CHIAMATI SOLO TRAMITE REFLECTION
    // private void Isaac()
    // {
    //     Debug.Log("Isaac");
    // }

    // private void FireFlower()
    // {
    //     Debug.Log("FireFlower");
    // }
}
