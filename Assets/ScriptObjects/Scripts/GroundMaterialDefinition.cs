using UnityEngine;

[CreateAssetMenu(fileName = "NewGroundMaterialDefinition", menuName = "Item System/GroundMaterial")]
public class GroundMaterialDefinition : ScriptableObject
{
    public GroundMaterial groundMaterial;
    public ItemId item;
    public Color color;
    public ItemId[] diggableBy;
    public ItemId[] fillableBy;
}
