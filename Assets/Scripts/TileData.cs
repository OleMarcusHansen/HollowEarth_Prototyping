using System.Collections.Generic;
using UnityEngine;

public class TileData
{
    public Vector3 position;
    public Vector3 rotation;
    public List<GroundMaterial> groundMaterial;
    public ItemData itemSlot;
    public GameObject lodObject;

    public TileData(Vector3 position, Vector3 rotation, GroundMaterial groundMaterial, ItemData itemData = null)
    {
        this.position = position;
        this.rotation = rotation;
        this.groundMaterial = new List<GroundMaterial>();
        this.groundMaterial.Add(groundMaterial);
        itemSlot = itemData;
    }

    public void SetGroundMaterial(GroundMaterial groundMaterial)
    {
        this.groundMaterial[0] = groundMaterial;
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
