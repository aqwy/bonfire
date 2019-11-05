using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBushCount : MonoBehaviour
{
    public Equippableitem halfSpikesBush;
    public Equippableitem fullSpikeBush;

    public int newItemCount
    {
        get
        {
            return _newItemCount;
        }

        set
        {
            _newItemCount = value;
        }
    }
    private int _newItemCount;

    public void checkNewItem()
    {
        if (_newItemCount == 1)
        {
            InventoryManager.Instance.inventory.addItem(halfSpikesBush);
            UIManager.Instance.newItemSignPanel.gameObject.SetActive(true);
        }
        else if (_newItemCount == 2)
        {
            InventoryManager.Instance.inventory.removeItem(halfSpikesBush);
            InventoryManager.Instance.inventory.addItem(fullSpikeBush);
            UIManager.Instance.newItemSignPanel.gameObject.SetActive(true);
        }
    }
}
