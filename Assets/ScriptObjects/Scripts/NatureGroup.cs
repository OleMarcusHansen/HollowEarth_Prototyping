using UnityEngine;

[CreateAssetMenu(fileName = "NewNatureGroup", menuName = "NatureGroup")]
public class NatureGroup : ScriptableObject
{
    public NatureSettings settings;
    public NatureFilter filter;
    public int groundMaterialMaxIndex;
    public NatureItem[] natureItems;

    public int biasTotal;

    [System.Serializable]
    public struct NatureItem
    {
        public ItemId item;
        public int bias;
    }
}
