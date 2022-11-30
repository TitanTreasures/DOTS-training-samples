using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;


public partial struct BeeSpawnerISystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
    
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        //EntityQuery beeEntityQuery = ecb.query.CreateEntityQuery(typeof(BeeTag));
        var beeSpawnerComponent = SystemAPI.GetSingleton<BeeSpawnerComponent>();

        new SpawnJob
        {
            BeePrefab = beeSpawnerComponent.beePrefab,
            ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged)
        }.Run();
    }
}
[BurstCompile]
public partial struct SpawnJob : IJobEntity
{
    public Entity BeePrefab;
    public EntityCommandBuffer ECB;

    [BurstCompile]
    public void Execute(SpawnBeeAspect spawnBeeAspect)
    {
        spawnBeeAspect.Spawn(ECB);
    }
}