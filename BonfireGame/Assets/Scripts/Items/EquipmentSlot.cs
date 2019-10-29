using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : ItemSlot
{
    public equipmentType equipmenttype;

    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = equipmenttype.ToString() + " slot";
    }
}
