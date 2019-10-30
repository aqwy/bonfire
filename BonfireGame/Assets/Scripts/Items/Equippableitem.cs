using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum equipmentType
{
    individual,
    special,
    all
}

public enum effectType
{
    slice,
    connect,
    crush
}

[CreateAssetMenu]
public class Equippableitem : Item
{
    public equipmentType equipmenttype;
    public effectType effecttype;
}

