using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct YellowBeeSpawnerComponent : IComponentData
{
    public Entity beePrefab;
    public int maxBeeSpawnCount;
    public float3 fieldDimensions;
}
