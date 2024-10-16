using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipeMap", menuName = "Recipe System/RecipeMap")]
public class RecipeMapping : ScriptableObject
{
    public Recipe[] recipes;

    public Recipe GetRecipe(ItemId hand, bool pull, ItemId ground)
    {
        foreach(Recipe recipe in recipes)
        {
            if (recipe.inputHand == hand && recipe.pull == pull && recipe.inputGround == ground)
            {
                return recipe;
            }
        }
        return null;
    }
}
