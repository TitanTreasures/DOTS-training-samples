using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class YellowBeeSpawnerMono : MonoBehaviour
{
    public GameObject beePrefab;
    public int maxBeeSpawnCount;
    public float3 fieldDimensions;
    // For randomness
    public uint randomSeed;
}
public class YellowBeeSpawnerBaker : Baker<YellowBeeSpawnerMono>
{
    public override void Bake(YellowBeeSpawnerMono authoring)
    {
        AddComponent(new YellowBeeSpawnerComponent
        {
            beePrefab = GetEntity(authoring.beePrefab),
            maxBeeSpawnCount = authoring.maxBeeSpawnCount,
            fieldDimensions = authoring.fieldDimensions
        });
        AddComponent(new RandomComponent
        {
            randomValue = Unity.Mathematics.Random.CreateFromIndex(authoring.randomSeed)
        });
    }
}