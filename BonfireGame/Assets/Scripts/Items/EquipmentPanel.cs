using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentPanel : MonoBehaviour
{
    [SerializeField] private Transform equpmentSlotParent;
    [SerializeField] private EquipmentSlot[] equipmentSlots;

    private Equippableitem _currentSlotItem;
    public Equippableitem CurrentSlotItem
    {
        get
        {
            return _currentSlotItem;
        }
        set
        {
            _currentSlotItem = value;
        }
    }

    private void OnValidate()
    {
        equipmentSlots = equpmentSlotParent.GetComponentsInChildren<EquipmentSlot>();
    }

    private void Start()
    {

    }

    public bool addItem(Equippableitem equippableitem, out Equippableitem previousItem)
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].equipmenttype == equippableitem.equipmenttype)
            {
                previousItem = (Equippableitem)equipmentSlots[i].Item;
                equipmentSlots[i].Item = equippableitem;
                _currentSlotItem = equippableitem;
                return true;
            }
        }
        previousItem = null;
        return false;
    }

    public bool removeItem(Equippableitem equippableitem)
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].Item == equippableitem)
            {
                equipmentSlots[i].Item = null;
                CurrentSlotItem = null;
                return true;
            }
        }
        return false;
    }

    public Equippableitem getCurrentItem()
    {
        if (!CurrentSlotItem)
        {
            return null;
        }
        else
        {
            return CurrentSlotItem;
        }
    }
}
