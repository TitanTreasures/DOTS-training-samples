using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct ResourceSpawnerComponent : IComponentData
{
    public Entity resourcePrefab;
    public int maxResourceSpawnCount;
    public float3 fieldDimensions;
}
