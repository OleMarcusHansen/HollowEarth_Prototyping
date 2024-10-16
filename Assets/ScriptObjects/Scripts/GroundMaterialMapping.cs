using UnityEngine;

[CreateAssetMenu(fileName = "NewGroundMaterialMap", menuName = "Item System/GroundMaterialMap")]
public class GroundMaterialMapping : ScriptableObject
{
    public GroundMaterialDefinition[] groundMaterialDefinitions;

    public GroundMaterialDefinition GetGroundMaterialDefinition(GroundMaterial groundMaterial)
    {
        foreach (GroundMaterialDefinition groundMaterialDefinition in groundMaterialDefinitions)
        {
            if (groundMaterial == groundMaterialDefinition.groundMaterial)
            {
                return groundMaterialDefinition;
            }
        }
        return null;
    }
    public GroundMaterialDefinition GetGroundMaterialDefinition(ItemId itemId)
    {
        foreach (GroundMaterialDefinition groundMaterialDefinition in groundMaterialDefinitions)
        {
            if (itemId == groundMaterialDefinition.item)
            {
                return groundMaterialDefinition;
            }
        }
        return null;
    }

    public bool IsDiggableBy(GroundMaterial groundMaterial, ItemId handItem)
    {
        foreach (ItemId i in GetGroundMaterialDefinition(groundMaterial).diggableBy)
        {
            if (handItem == i)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsFillableBy(ItemId itemId, ItemId handItem)
    {
        GroundMaterialDefinition groundMaterialDefinition = GetGroundMaterialDefinition(itemId);

        if (groundMaterialDefinition == null)
        {
            return false;
        }

        foreach (ItemId i in groundMaterialDefinition.fillableBy)
        {
            if (handItem == i)
            {
                return true;
            }
        }
        return false;
    }
}
