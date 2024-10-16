using UnityEngine;

public class SlotObject : MonoBehaviour
{
    public ItemObject itemSlot;
    [SerializeField] float slotHeight = 0f;

    public ItemObject SpawnItem(GameObject obj, int rotation = 0)
    {
        float parentYRotation = 0;
        if (!GetComponent<TileObject>())
        {
            parentYRotation = transform.localEulerAngles.y;
        }

        if (itemSlot != null)
        {
            ItemObject tmp = itemSlot;
            itemSlot = Instantiate(obj, transform).GetComponent<ItemObject>();
            itemSlot.transform.localEulerAngles += new Vector3(0, rotation - parentYRotation, 0);
            tmp.transform.parent = itemSlot.transform;
            itemSlot.itemSlot = tmp;
            itemSlot.transform.localPosition += new Vector3(0, slotHeight, 0);
        }
        else
        {
            itemSlot = Instantiate(obj, transform).GetComponent<ItemObject>();
            itemSlot.transform.localPosition += new Vector3(0, slotHeight, 0);
            itemSlot.transform.localEulerAngles += new Vector3(0, rotation - parentYRotation, 0);
        }
        return itemSlot;
    }
    public void DespawnItem()
    {
        if (itemSlot != null)
        {
            if (itemSlot.itemSlot != null)
            {
                ItemObject tmp = itemSlot.itemSlot;
                tmp.transform.parent = transform;
                Destroy(itemSlot.gameObject);
                itemSlot = tmp;
            }
            else
            {
                Destroy(itemSlot.gameObject);
                itemSlot = null;
            }
        }
    }
}
