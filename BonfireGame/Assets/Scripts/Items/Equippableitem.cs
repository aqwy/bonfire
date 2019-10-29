using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum equipmentType
{
    slice,
    connect,
    crush
}

[CreateAssetMenu]
public class Equippableitem : Item
{
    public equipmentType equipmenttype;
}

