using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "NewItemMap", menuName = "Item System/ItemMap")]
public class ItemMapping : ScriptableObject
{
    [SerializeField] bool loadItems;

    public ItemDefinition[] itemDefinitions;

    //public ItemMap[] itemMap;

    private void OnValidate()
    {
        if (loadItems == true)
        {
            LoadItems();

            loadItems = false;
        }
    }

    void LoadItems()
    {
        itemDefinitions = Resources.LoadAll<ItemDefinition>("Items").OrderBy(item => (int)item.itemId).ToArray();
    }

    public ItemDefinition GetItemDefinition(ItemId itemId)
    {
        if (itemDefinitions[(int)itemId - 1].itemId == itemId)
        {
            return itemDefinitions[(int)itemId - 1];
        }

        Debug.LogWarning("Item not found with direct access, using fallback");

        foreach (ItemDefinition itemDefinition in itemDefinitions)
        {
            if (itemId == itemDefinition.itemId)
            {
                return itemDefinition;
            }
        }

        Debug.LogWarning("Item not found with fallback, returning null");

        return null;
    }

    /*
    public GameObject RetrieveItemPrefab(ItemId item)
    {
        foreach (ItemMap itemPrefabMap in itemMap) // kan bytte til binærsøk hvis den er alfabetisk
        {
            if (item == itemPrefabMap.item)
            {
                return itemPrefabMap.prefab;
            }
        }
        Debug.LogWarning("Item not found");
        return null;
    }
    public GameObject RetrieveLodPrefab(ItemId item)
    {
        foreach (ItemMap itemPrefabMap in itemMap) // kan bytte til binærsøk hvis den er alfabetisk
        {
            if (item == itemPrefabMap.item)
            {
                return itemPrefabMap.lodPrefab;
            }
        }
        Debug.LogWarning("Item not found");
        return null;
    }
    public bool RetrievePickupable(ItemId item)
    {
        foreach (ItemMap itemPrefabMap in itemMap) // kan bytte til binærsøk hvis den er alfabetisk
        {
            if (item == itemPrefabMap.item)
            {
                return itemPrefabMap.pickupable;
            }
        }
        Debug.LogWarning("Item not found");
        return false;
    }
    */
}
/*
[System.Serializable]
public struct ItemMap
{
    public ItemId item;
    public GameObject prefab;
    public GameObject lodPrefab;
    public bool pickupable;
}*/