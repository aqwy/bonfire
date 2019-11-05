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
    nothing,
    activator,
    slice,  
    connect,
    crush,
    shoot,
    explosion
}

[CreateAssetMenu]
public class Equippableitem : Item
{
    public equipmentType equipmenttype;
    public effectType effecttype;
}

