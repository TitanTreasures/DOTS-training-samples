using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.PackageManager;
using UnityEngine;
using static ResourcePositionBufferSystem;
using static UnityEngine.EventSystems.EventTrigger;

[BurstCompile]
public partial struct MoveSystem : ISystem
{
    public Unity.Mathematics.Random random;

    EntityQuery seekingQuery;
    EntityQuery carryingQuery;
    EntityQuery resourceBeingCarriedQuery;
    EntityQuery attackingQuery;
    EntityQuery resourceFollowQuery;


    
    Entity e;
    EntityQuery resourceQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = Unity.Mathematics.Random.CreateFromIndex(1);
        seekingQuery = state.GetEntityQuery(ComponentType.ReadOnly<BeeSeekingTag>());
        carryingQuery = state.GetEntityQuery(ComponentType.ReadOnly<BeeCarryingTag>());
        attackingQuery = state.GetEntityQuery(ComponentType.ReadOnly<BeeAttackingTag>());
        resourceBeingCarriedQuery = state.GetEntityQuery(ComponentType.ReadOnly<ResourceBeingCarriedTag>());
        resourceQuery = state.GetEntityQuery(ComponentType.ReadOnly<TargetResourceComponent>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        new BeeSeekingJob
        {
            DeltaTime = deltaTime,
            ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel(seekingQuery);

        new BeeCarryingJob
        {
            DeltaTime = deltaTime,
            ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel(carryingQuery);

        new ResourceFollowJob
        {
            DeltaTime = deltaTime,
            ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel(resourceBeingCarriedQuery);
    }

    [BurstCompile]
    public partial struct BeeSeekingJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(BeeAspect bee, [EntityIndexInQuery] int sortKey)
        {
            bee.MoveTo(DeltaTime);
            if (bee.IsInPickupRange())
            {
                ECB.SetComponentEnabled<BeeReadyToPickupTag>(sortKey, bee.entity, true);
                ECB.SetComponentEnabled<BeeSeekingTag>(sortKey, bee.entity, false);
            }
        }
    }

    // TODO: LAV DET HER
    [BurstCompile]
    public partial struct BeeCarryingJob : IJobEntity
    {
        public float DeltaTime;
        public float3 BasePos;
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(BeeAspect bee, [EntityIndexInQuery] int sortKey)
        {
            bee.MoveToBase(DeltaTime);
            if (bee.IsInSpawnLocationRange())
            {
                ECB.SetComponentEnabled<BeeCarryingTag>(sortKey, bee.entity, false);
                ECB.SetComponentEnabled<BeeIdleTag>(sortKey, bee.entity, true);
            }
        }
    }
    [BurstCompile]
    public partial struct ResourceFollowJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(ResourceAspect resource)
        {
            resource.FollowTarget();
        }
    }
}
