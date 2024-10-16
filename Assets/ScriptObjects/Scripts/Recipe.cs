using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Recipe System/Recipe")]
public class Recipe : ScriptableObject
{
    public ItemId inputHand;
    public ItemId inputGround;
    public bool pull;
    public ItemId outputHand;
    public ItemId[] outputGround;
    public bool keepRotation;
    public bool blocked;
}