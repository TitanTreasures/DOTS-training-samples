using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct ResourcePropertiesComponent : IComponentData
{
    public Entity currentBeeHolder;
    public float3 currentBeeHolderPosition;
    public float droppingSpeed;
}
