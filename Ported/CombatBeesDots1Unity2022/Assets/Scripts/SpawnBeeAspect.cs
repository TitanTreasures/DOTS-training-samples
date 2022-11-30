using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct SpawnBeeAspect : IAspect
{
    private readonly Entity entity;
    private readonly TransformAspect transformAspect;
    private readonly RefRO<MaxSpawnCount> maxBeeSpawn;
    private readonly RefRW<CurrentBeeSpawnCount> currentBeeSpawnCount;

    public void Spawn(EntityCommandBuffer ECB)
    {
        if (currentBeeSpawnCount.ValueRO.value < maxBeeSpawn.ValueRO.value)
        {
            ECB.Instantiate(entity);
            currentBeeSpawnCount.ValueRW.value++;
        }
    }
}
