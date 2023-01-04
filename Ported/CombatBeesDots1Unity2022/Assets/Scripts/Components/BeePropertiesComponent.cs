using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BeePropertiesComponent : IComponentData
{
    public float flySpeed;
    public float3 enemyTargetPosition;
    public float resourceInteractionRange;
    public float3 basePosition;
    public float attackRadius;
}
