using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BeeTargetPositionComponent : IComponentData
{
    public float3 targetPosition;
}
