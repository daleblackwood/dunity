using UnityEngine;

[System.Serializable]
public class ItemCounter
{
    public int[] itemCounts = new int[(int)ItemType.MAX];

    public int GetCount(ItemType itemType)
    {
        int index = (int)itemType;
        return itemCounts[index];
    }

    public void SetCount(ItemType itemType, int count)
    {
        int index = (int)itemType;
        itemCounts[index] = count;
    }

    public int AddItem(ItemType itemType, int increase = 1)
    {
        int newCount = GetCount(itemType) + increase;
        if (newCount < 0)
        {
            newCount = 0;
        }
        SetCount(itemType, newCount);
        return newCount;
    }

    public void Reset()
    {
        for (int i = 0; i < (int)ItemType.MAX; i++)
        {
            itemCounts[i] = 0;
        }
    }

    public void Copy(ItemCounter other)
    {
        for (int i=0; i<(int)ItemType.MAX; i++)
        {
            ItemType itemType = (ItemType)i;
            int count = other.GetCount(itemType);
            SetCount(itemType, count);
        }
    }
}