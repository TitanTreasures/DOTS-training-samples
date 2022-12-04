using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
// This is done to make this system part of the group that is handled before others
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SpawnBeeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeSpawnerComponent>();
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var beeSpawnerEntity = SystemAPI.GetSingletonEntity<BeeSpawnerComponent>();
        var beeSpawnerAspect = SystemAPI.GetAspectRW<BeeSpawnerAspect>(beeSpawnerEntity);

        // Using temp for the ecb, because it is cheapest (Disposes at the same frame)
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        for (int i = 0; i < beeSpawnerAspect.maxBeeSpawnCount; i++)
        {
            var newBee = ecb.Instantiate(beeSpawnerAspect.beePrefab);
            var newTransform = beeSpawnerAspect.GetRandomBeeTransform();
            ecb.SetComponent(newBee, new LocalToWorldTransform { Value = newTransform });
        }

        ecb.Playback(state.EntityManager);
        state.Enabled = false;
    }
}
