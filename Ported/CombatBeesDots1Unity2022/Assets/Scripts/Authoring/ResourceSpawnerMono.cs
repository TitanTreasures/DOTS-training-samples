using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ResourceSpawnerMono : MonoBehaviour
{
    public GameObject resourcePrefab;
    public int maxResourceSpawnCount;
    public float3 fieldDimensions;
    // For randomness
    public uint randomSeed;
}
public class ResourceSpawnerBaker : Baker<ResourceSpawnerMono>
{
    public override void Bake(ResourceSpawnerMono authoring)
    {
        AddComponent(new ResourceSpawnerComponent
        {
            resourcePrefab = GetEntity(authoring.resourcePrefab),
            maxResourceSpawnCount = authoring.maxResourceSpawnCount,
            fieldDimensions = authoring.fieldDimensions
        });
        AddComponent(new RandomComponent
        {
            randomValue = Unity.Mathematics.Random.CreateFromIndex(authoring.randomSeed)
        });
    }
}

