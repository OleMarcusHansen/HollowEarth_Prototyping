using UnityEngine;

public class TileObject : SlotObject
{
    public int id;

    public void ResetTile()
    {
        if (itemSlot != null)
        {
            Destroy(itemSlot.gameObject);
            itemSlot = null;
        }
    }
}
