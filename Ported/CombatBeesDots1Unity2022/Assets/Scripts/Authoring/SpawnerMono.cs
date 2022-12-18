using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpawnerMono : MonoBehaviour
{
    public GameObject resourcePrefab;
    public GameObject blueBeePrefab;
    public GameObject yellowBeePrefab;
    public int resourceSpawnCount, blueBeeSpawnCount, yellowBeeSpawnCount;
    public float3 resourceFieldPosition, blueBeeFieldPosition, yellowBeeFieldPosition;
    public float3 resourceFieldDimensions, blueBeeFieldDimensions, yellowBeeFieldDimensions;
    // For randomness
    public uint randomSeed;
}
public class SpawnerBaker : Baker<SpawnerMono>
{
    public override void Bake(SpawnerMono authoring)
    {
        AddComponent(new SpawnerComponent
        {
            resourcePrefab = GetEntity(authoring.resourcePrefab),
            resourceSpawnCount = authoring.resourceSpawnCount,
            resourceFieldPosition = authoring.resourceFieldPosition,
            resourceFieldDimensions = authoring.resourceFieldDimensions,

            blueBeePrefab = GetEntity(authoring.blueBeePrefab),
            blueBeeSpawnCount = authoring.blueBeeSpawnCount,
            blueBeeFieldPosition = authoring.blueBeeFieldPosition,
            blueBeeFieldDimensions = authoring.blueBeeFieldDimensions,

            yellowBeePrefab = GetEntity(authoring.yellowBeePrefab),
            yellowBeeSpawnCount = authoring.yellowBeeSpawnCount,
            yellowBeeFieldPosition = authoring.yellowBeeFieldPosition,
            yellowBeeFieldDimensions = authoring.yellowBeeFieldDimensions
        });
        AddComponent(new RandomComponent
        {
            randomValue = Unity.Mathematics.Random.CreateFromIndex(authoring.randomSeed)
        });
    }
}