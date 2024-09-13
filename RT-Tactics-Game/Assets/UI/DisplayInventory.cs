using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayInventory : MonoBehaviour
{
    public InventoryObject inventory; // The current inventory to display
    public int X_START;
    public int Y_START;
    public int X_SPACE_BETWEEN_ITEM;
    public int NUMBER_OF_COLUMN;
    public int Y_SPACE_BETWEEN_ITEM;

    private Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();

    public void UpdateMenu(InventoryObject newInventory)
    {
        if (inventory != newInventory)
        {
            ClearDisplay(); // Clear the current inventory display
            inventory = newInventory; // Set the new inventory to display
            CreateDisplay(); // Re-create the display for the new inventory
        }
    }

    public void CreateDisplay()
    {
        if (inventory == null) return;

        for (int i = 0; i < inventory.Container.Count; i++)
        {
            var slot = inventory.Container[i];
            var itemObject = slot.item.prefab;

            if (itemsDisplayed.ContainsKey(slot))
            {
                // Update the existing display item
                var obj = itemsDisplayed[slot];
                obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
            }
            else
            {
                // Create a new display item
                var obj = Instantiate(itemObject, Vector3.zero, Quaternion.identity, transform);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
                itemsDisplayed.Add(slot, obj);
            }
        }
    }

    public void ClearDisplay()
    {
        foreach (var item in itemsDisplayed.Values)
        {
            Destroy(item);
        }
        itemsDisplayed.Clear();
    }

    private Vector3 GetPosition(int i)
    {
        return new Vector3(X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMN)), Y_START + (Y_SPACE_BETWEEN_ITEM * (i / NUMBER_OF_COLUMN)), 0f);
    }
}
