using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SpawnerComponent : IComponentData
{
    public Entity resourcePrefab;
    public Entity blueBeePrefab;
    public Entity yellowBeePrefab;
    public int resourceSpawnCount, blueBeeSpawnCount, yellowBeeSpawnCount;
    public float3 resourceFieldPosition, blueBeeFieldPosition, yellowBeeFieldPosition;
    public float3 resourceFieldDimensions, blueBeeFieldDimensions, yellowBeeFieldDimensions;
    // For randomness
    public uint randomSeed;
}
