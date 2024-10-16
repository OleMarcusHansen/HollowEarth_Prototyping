using System.Collections.Generic;
using UnityEngine;

public class ItemData
{
    public ItemId item;
    public int rotation;
    public ItemData itemSlot;

    public ItemData(ItemId item, int rotation = 0, ItemData itemData = null)
    {
        this.item = item;
        this.rotation = rotation;
        itemSlot = itemData;
    }

    public ItemData GetItem(int depth)
    {
        if (itemSlot != null)
        {
            if (depth == 0)
            {
                return itemSlot;
            }
            else
            {
                return itemSlot.GetItem(depth - 1);
            }
        }
        else
        {
            return null;
        }
    }
    public void AddItem(int depth, ItemData itemData)
    {
        if (depth == 0)
        {
            if (itemSlot != null)
            {
                ItemData tmp = itemSlot;
                itemSlot = itemData;
                itemSlot.itemSlot = tmp;
            }
            else
            {
                itemSlot = itemData;
            }
        }
        else
        {
            if (itemSlot != null)
            {
                itemSlot.AddItem(depth - 1, itemData);
            }
        }
    }
    public void RemoveItem(int depth)
    {
        if (itemSlot != null)
        {
            if (depth == 0)
            {
                if (itemSlot.itemSlot != null)
                {
                    itemSlot = itemSlot.itemSlot;
                }
                else
                {
                    itemSlot = null;
                }
            }
            else
            {
                itemSlot.RemoveItem(depth - 1);
            }
        }
        else
        {
            Debug.LogWarning("Tried remove too deep");
        }
    }
}
