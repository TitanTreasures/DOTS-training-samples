using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;


[BurstCompile]
// This is done to make this system part of the group that is handled before others
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct InitialSpawnerSystem : ISystem
{
    Entity bee;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Disabling the system ensures it runs only once... For some reason...
        state.RequireForUpdate<SpawnerComponent>();
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        // Using temp for the ecb, because it is cheapest (Disposes at the same frame)
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var spawnerEntity = SystemAPI.GetSingletonEntity<SpawnerComponent>();
        var spawnerAspect = SystemAPI.GetAspectRW<SpawnerAspect>(spawnerEntity);

        for (int i = 0; i < spawnerAspect.resourceSpawnCount; i++)
        {
            Entity entity = ecb.Instantiate(spawnerAspect.resourceSpawnPrefab);
            var newTransform = spawnerAspect.GetSpawnTransform(spawnerAspect.resourceSpawnPrefab);
            ecb.SetComponent(entity, new LocalTransform { Position = newTransform.Position, Rotation = newTransform.Rotation, Scale = newTransform.Scale });
        }

        for (int i = 0; i < spawnerAspect.blueBeeSpawnCount; i++)
        {
            Entity entity = ecb.Instantiate(spawnerAspect.blueBeeSpawnPrefab);
            var newTransform = spawnerAspect.GetSpawnTransform(spawnerAspect.blueBeeSpawnPrefab);
            ecb.SetComponent(entity, new LocalTransform { Position = newTransform.Position, Rotation = newTransform.Rotation, Scale = newTransform.Scale });
            ecb.SetComponent(entity, new RandomComponent { randomValue = Unity.Mathematics.Random.CreateFromIndex(Convert.ToUInt32(i)) });
            ecb.SetComponentEnabled(entity, typeof(BeeCarryingTag), false);
            ecb.SetComponentEnabled(entity, typeof(BeeAttackingTag), false);
            //ecb.SetComponentEnabled(entity, typeof(BeeIdleTag), true);
        }

        for (int i = 0; i < spawnerAspect.yellowBeeSpawnCount; i++)
        {
            Entity entity = ecb.Instantiate(spawnerAspect.yellowBeeSpawnPrefab);
            var newTransform = spawnerAspect.GetSpawnTransform(spawnerAspect.yellowBeeSpawnPrefab);
            ecb.SetComponent(entity, new LocalTransform { Position = newTransform.Position, Rotation = newTransform.Rotation, Scale = newTransform.Scale });
            ecb.SetComponent(entity, new RandomComponent { randomValue = Unity.Mathematics.Random.CreateFromIndex(Convert.ToUInt32(i)) });
            ecb.SetComponentEnabled(entity, typeof(BeeCarryingTag), false);
            ecb.SetComponentEnabled(entity, typeof(BeeAttackingTag), false);
            //ecb.SetComponentEnabled(entity, typeof(BeeIdleTag), true);
        }
        ecb.Playback(state.EntityManager);

    }
}
