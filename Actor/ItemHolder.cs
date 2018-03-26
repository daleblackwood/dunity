using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour 
{
    public Item startingItem;
    public Item item { get; private set; }
    public System.Action OnItemChange = null;

    void OnEnable()
    {
        if (startingItem != null)
        {
            SetItem(startingItem);
        }
    }

    void Update()
    {
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale = Vector3.one;
    }

    public void SetItem(Item newItem)
    {
        if (item != null)
        {
            Utils.Remove(item);
            item = null;
        }

        newItem.gameObject.SetActive(false);

        item = Utils.InstantiateInto(newItem, transform);
        item.gameObject.SetActive(true);

        if (OnItemChange != null)
        {
            OnItemChange.Invoke();
        }
    }
}
