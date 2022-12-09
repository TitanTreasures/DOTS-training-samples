using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


[BurstCompile]
// This is done to make this system part of the group that is handled before others
[UpdateInGroup(typeof(InitializationSystemGroup))]
// Maybe rename to "InitialSpawnerSystem" or something like that
public partial struct SpawnBeeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BlueBeeSpawnerComponent>();
        state.RequireForUpdate<YellowBeeSpawnerComponent>();
        state.RequireForUpdate<ResourceSpawnerComponent>();
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Disabling the system ensures it runs only once... For some reason...
        state.Enabled = false;
        // Using temp for the ecb, because it is cheapest (Disposes at the same frame)
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Blue teams initial spawning
        var blueBeeSpawnerEntity = SystemAPI.GetSingletonEntity<BlueBeeSpawnerComponent>();
        var blueBeeSpawnerAspect = SystemAPI.GetAspectRW<BlueBeeSpawnerAspect>(blueBeeSpawnerEntity);
        for (int i = 0; i < blueBeeSpawnerAspect.maxBeeSpawnCount; i++)
        {
            // Set create new bee and set initial position
            var newBee = ecb.Instantiate(blueBeeSpawnerAspect.beePrefab);
            var newTransform = blueBeeSpawnerAspect.GetRandomBeeTransform();
            ecb.SetComponent(newBee, new LocalToWorldTransform { Value = newTransform });
        }

        // Yellow teams initial spawning
        var yellowBeeSpawnerEntity = SystemAPI.GetSingletonEntity<YellowBeeSpawnerComponent>();
        var yellowBeeSpawnerAspect = SystemAPI.GetAspectRW<YellowBeeSpawnerAspect>(yellowBeeSpawnerEntity);
        for (int i = 0; i < yellowBeeSpawnerAspect.maxBeeSpawnCount; i++)
        {
            // Set create new bee and set initial position
            var newBee = ecb.Instantiate(yellowBeeSpawnerAspect.beePrefab);
            var newTransform = yellowBeeSpawnerAspect.GetRandomBeeTransform();
            ecb.SetComponent(newBee, new LocalToWorldTransform { Value = newTransform });
        }

        // Resources initial spawning
        var resourceSpawnerEntity = SystemAPI.GetSingletonEntity<ResourceSpawnerComponent>();
        var resourceSpawnerAspect = SystemAPI.GetAspectRW<ResourceSpawnerAspect>(resourceSpawnerEntity);
        for (int i = 0; i < resourceSpawnerAspect.maxResourceSpawnCount; i++)
        {
            // Set create new bee and set initial position
            var newResource = ecb.Instantiate(resourceSpawnerAspect.resourcePrefab);
            var newTransform = resourceSpawnerAspect.GetRandomResourceTransform();
            ecb.SetComponent(newResource, new LocalToWorldTransform { Value = newTransform });
        }

        ecb.Playback(state.EntityManager);
    }
}
