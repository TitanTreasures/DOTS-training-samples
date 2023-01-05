using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using static MoveSystem;
using static UnityEngine.EventSystems.EventTrigger;


[BurstCompile]
public partial struct SpawnSystem : ISystem
{
    EntityQuery resourceDespawnQuery;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        resourceDespawnQuery = state.GetEntityQuery(ComponentType.ReadOnly<ResourceDespawnTag>());
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        new ResourceDespawnJob
        {
            
            ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel(resourceDespawnQuery);

    }

    [BurstCompile]
    public partial struct ResourceDespawnJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(ResourceAspect resource, [EntityIndexInQuery] int sortKey)
        {
            ECB.DestroyEntity(sortKey, resource.entity);
                ECB.SetComponentEnabled<BeeReadyToPickupTag>(sortKey, resource.entity, true);
                ECB.SetComponentEnabled<BeeSeekingTag>(sortKey, resource.entity, false);
        }
    }
}
