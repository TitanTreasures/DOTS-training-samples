using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpawnerMono : MonoBehaviour
{
    [Header("Seed")]
    // For randomness
    public uint randomSeed;
    [Header("Resource")]
    public GameObject resourcePrefab;
    public int resourceSpawnCount;
    public float3 resourceFieldPosition;
    public float3 resourceFieldDimensions;
    [Header("Blue Bee")]
    public GameObject blueBeePrefab;
    public int blueBeeSpawnCount; 
    public float3 blueBeeFieldPosition;
    public float3 blueBeeFieldDimensions;
    [Header("Yellow Bee")]
    public GameObject yellowBeePrefab;
    public int yellowBeeSpawnCount;
    public float3 yellowBeeFieldPosition;
    public float3 yellowBeeFieldDimensions;
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