using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using static UnityEngine.EventSystems.EventTrigger;


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
            SetResourceTagComponents(ecb,entity);
        }

        for (int i = 0; i < spawnerAspect.blueBeeSpawnCount; i++)
        {
            Entity entity = ecb.Instantiate(spawnerAspect.blueBeeSpawnPrefab);
            var newTransform = spawnerAspect.GetSpawnTransform(spawnerAspect.blueBeeSpawnPrefab);
            ecb.SetComponent(entity, new LocalTransform { Position = newTransform.Position, Rotation = newTransform.Rotation, Scale = newTransform.Scale });
            ecb.SetComponent(entity, new RandomComponent { randomValue = Unity.Mathematics.Random.CreateFromIndex(Convert.ToUInt32(i)) });
            ecb.AddComponent(entity, new BeeSpawnLocationComponent { basePosition = newTransform.Position });
            SetBeeTagComponents(ecb, entity);
        }

        for (int i = 0; i < spawnerAspect.yellowBeeSpawnCount; i++)
        {
            Entity entity = ecb.Instantiate(spawnerAspect.yellowBeeSpawnPrefab);
            var newTransform = spawnerAspect.GetSpawnTransform(spawnerAspect.yellowBeeSpawnPrefab);
            ecb.SetComponent(entity, new LocalTransform { Position = newTransform.Position, Rotation = newTransform.Rotation, Scale = newTransform.Scale });
            ecb.SetComponent(entity, new RandomComponent { randomValue = Unity.Mathematics.Random.CreateFromIndex(Convert.ToUInt32(i)) });
            ecb.AddComponent(entity, new BeeSpawnLocationComponent { basePosition = newTransform.Position });
            SetBeeTagComponents(ecb,entity);
        }

        // DEBUG SECTION -----------------------------------------------
        //for (int i = 0; i < spawnerAspect.yellowBeeSpawnCount; i++)
        //{
        //    Entity parentEntity = ecb.Instantiate(spawnerAspect.yellowBeeSpawnPrefab);
        //    var newTransform1 = spawnerAspect.GetSpawnTransform(spawnerAspect.yellowBeeSpawnPrefab);
        //    ecb.SetComponent(parentEntity, new LocalTransform { Position = newTransform1.Position, Rotation = newTransform1.Rotation, Scale = newTransform1.Scale });
        //    ecb.SetComponent(parentEntity, new RandomComponent { randomValue = Unity.Mathematics.Random.CreateFromIndex(Convert.ToUInt32(i)) });
        //    ecb.SetComponentEnabled(parentEntity, typeof(BeeCarryingTag), true);
        //    ecb.SetComponentEnabled(parentEntity, typeof(BeeAttackingTag), false);

        //    Entity childEntity = ecb.Instantiate(spawnerAspect.resourceSpawnPrefab);
        //    //var newTransform = spawnerAspect.GetSpawnTransform(spawnerAspect.resourceSpawnPrefab);
        //    ecb.SetComponent(childEntity, new LocalTransform { Position = newTransform1.Position, Rotation = newTransform1.Rotation, Scale = newTransform1.Scale });
        //    //ecb.SetComponent(childEntity, new ResourcePropertiesComponent { currentBeeHolderPosition = newTransform1.Position });
        //    ecb.SetComponentEnabled(childEntity, typeof(ResourceBeingCarriedTag), true); 
        //}

        // DEBUG SECTION -----------------------------------------------

        ecb.Playback(state.EntityManager);

    }

    public void SetBeeTagComponents(EntityCommandBuffer ecb, Entity entity) {
        ecb.AddComponent(entity, ComponentType.ReadOnly<BeeIdleTag>());
        ecb.AddComponent(entity, ComponentType.ReadOnly<BeeSeekingTag>());
        ecb.AddComponent(entity, ComponentType.ReadOnly<BeeCarryingTag>());
        ecb.AddComponent(entity, ComponentType.ReadOnly<BeeAttackingTag>());
        ecb.AddComponent(entity, ComponentType.ReadOnly<BeeReadyToPickupTag>());

        ecb.SetComponentEnabled(entity, ComponentType.ReadOnly<BeeIdleTag>(), true);
        ecb.SetComponentEnabled(entity, ComponentType.ReadOnly<BeeSeekingTag>(), false);
        ecb.SetComponentEnabled(entity, ComponentType.ReadOnly<BeeCarryingTag>(), false);
        ecb.SetComponentEnabled(entity, ComponentType.ReadOnly<BeeAttackingTag>(), false);
        ecb.SetComponentEnabled(entity, ComponentType.ReadOnly<BeeReadyToPickupTag>(), false);
    }

    public void SetResourceTagComponents(EntityCommandBuffer ecb, Entity entity)
    {
        ecb.AddComponent(entity, ComponentType.ReadOnly<ResourceTag>());
        ecb.AddComponent(entity, ComponentType.ReadOnly<ResourceBeingCarriedTag>());
        ecb.AddComponent(entity, ComponentType.ReadOnly<ResourceReadyForPickUpTag>());
        ecb.AddComponent(entity, ComponentType.ReadOnly<ResourceDoesNotExistInBufferTag>());
        ecb.AddComponent(entity, ComponentType.ReadOnly<ResourceDroppingTag>());
        ecb.AddComponent(entity, ComponentType.ReadOnly < ResourceDespawnTag>());

        ecb.SetComponentEnabled(entity, ComponentType.ReadOnly<ResourceTag>(), true);
        ecb.SetComponentEnabled(entity, ComponentType.ReadOnly<ResourceBeingCarriedTag>(), false);
        ecb.SetComponentEnabled(entity, ComponentType.ReadOnly<ResourceReadyForPickUpTag>(), false);
        ecb.SetComponentEnabled(entity, ComponentType.ReadOnly<ResourceDoesNotExistInBufferTag>(), false);
        ecb.SetComponentEnabled(entity, ComponentType.ReadOnly<ResourceDroppingTag>(), true);
        ecb.SetComponentEnabled(entity, ComponentType.ReadOnly<ResourceDespawnTag>(), false);
    }
}
