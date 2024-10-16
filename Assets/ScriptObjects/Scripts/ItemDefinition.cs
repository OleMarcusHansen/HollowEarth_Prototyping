using UnityEngine;

[CreateAssetMenu(fileName = "NewItemDefinition", menuName = "Item System/ItemDefinition")]
public class ItemDefinition : ScriptableObject
{
    public ItemId itemId;
    public GameObject prefab;
    public GameObject lodPrefab;
    public bool pickupable;
}
