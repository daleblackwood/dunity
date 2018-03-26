using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour 
{
    Actor actor = null;

    void Awake()
    {
        actor = GetComponent<Actor>();
    }

    public int GetItemCount(ItemType itemType)
    {
        return GetCounter().GetCount(itemType);
    }

    public int AddItem(ItemType itemType, int increase = 1)
    {
        return GetCounter().AddItem(itemType, increase);
    }

    public void SetItemCount(ItemType itemType, int value)
    {
        GetCounter().SetCount(itemType, value);
    }

    public ItemCounter GetCounter()
    {
        return new ItemCounter();
        //return Game.Instance.saver.saveGame.itemCounter;
    }

    void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<Item>();
        if (item == null || item.enabled == false)
            return;

        if (item.itemType == ItemType.Heart)
        {
            if (actor.health >= actor.maxHealth)
                return;

            actor.health += item.count;
        }

        AddItem(item.itemType);
        item.Collect(this);
    }
}
