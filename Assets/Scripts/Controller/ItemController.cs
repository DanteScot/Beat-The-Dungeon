using System;
using System.Collections.Generic;
using UnityEngine;

// Enumeratore che contiene tutti gli oggetti che possono essere raccolti
public enum ItemSelected{
    Random,
    Isaac,
    Jack,
    FireFlower,
    // TODO: implements
    // Amplifier
}

// Classe che gestisce il comportamento degli oggetti raccolti
public class ItemController : PickUp
{
    private static List<ItemSelected> availableItems;

    public ItemSelected itemSelected;
    [SerializeField] private GameObject gear;

    // Resources path
    private string basePath = "Sprites/";
    private string selected;


    public new void Start()
    {
        base.Start();

        // Se la lista degli oggetti disponibili non è stata inizializzata, la inizializza
        if(availableItems == null){
            availableItems = new List<ItemSelected>();

            foreach (ItemSelected item in Enum.GetValues(typeof(ItemSelected)))
            {
                if(item == ItemSelected.Random) continue;
                availableItems.Add(item);
            }
        }

        // Se non ci sono più oggetti disponibili, istanzia l'ingranaggio e distrugge l'oggetto
        if(availableItems.Count == 0){
            transform.parent.parent.parent.parent.GetComponentInChildren<RoomManager>().SetReward(gear);
            Destroy(gameObject);
            return;
        }
        
        // Se l'oggetto selezionato è Random, ne seleziona uno a caso
        if(itemSelected == ItemSelected.Random) itemSelected = availableItems[UnityEngine.Random.Range(0, availableItems.Count)];

        // Rimuove l'oggetto selezionato dalla lista degli oggetti disponibili e selected contiene il nome dell'oggetto selezionato
        availableItems.Remove(itemSelected);
        selected = Enum.GetName(typeof(ItemSelected), itemSelected);
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(basePath + selected);
    }

    // Metodo che viene chiamato quando l'oggetto viene raccolto
    // Aggiunge l'oggetto selezionato al giocatore e notifica l'evento
    public override void PickUpItem()
    {
        PlayerManager.Instance.GetPlayer().GetComponent<PlayerController>().AddPower(selected);
        Messenger<string>.Broadcast(GameEvent.ITEM_PICKED, selected);
    }
}
