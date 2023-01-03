using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BeePropertiesComponent : IComponentData
{
    public float flySpeed;
    public Entity enemyTarget;
    public float pickupRange;
}
