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

[BurstCompile]
public partial struct BeeToResourceSystem : ISystem
{
    public Unity.Mathematics.Random random;

    EntityQuery seekingQuery;
    EntityQuery carryingQuery;
    EntityQuery attackingQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = Unity.Mathematics.Random.CreateFromIndex(1);
        seekingQuery = state.GetEntityQuery(ComponentType.ReadOnly<BeeSeekingTag>());
        carryingQuery = state.GetEntityQuery(ComponentType.ReadOnly<BeeCarryingTag>());
        attackingQuery = state.GetEntityQuery(ComponentType.ReadOnly<BeeAttackingTag>());
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
            ResourcePos = float3.zero,
            ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel(seekingQuery);

        new BeeCarryingJob
        {
            DeltaTime = deltaTime,
            BasePos = new float3(20, 0, 0),
            ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel(carryingQuery);

    }

    [BurstCompile]
    public partial struct BeeSeekingJob : IJobEntity
    {
        public float DeltaTime;
        public float3 ResourcePos;
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(BeeMoveToResourceAspect bee, [EntityIndexInQuery] int sortKey)
        {
            bee.FlyToResource(DeltaTime, ResourcePos);
            if (bee.IsInPickupRange(ResourcePos, 1f))
            {
                ECB.SetComponentEnabled<BeeCarryingTag>(sortKey, bee.entity, true);
                ECB.SetComponentEnabled<BeeSeekingTag>(sortKey, bee.entity, false);
            }
        }
    }
    [BurstCompile]
    public partial struct BeeCarryingJob : IJobEntity
    {
        public float DeltaTime;
        public float3 BasePos;
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(BeeMoveToResourceAspect bee, [EntityIndexInQuery] int sortKey)
        {
            bee.FlyToResource(DeltaTime, BasePos);
            if (bee.IsInPickupRange(BasePos, 1f))
            {
                ECB.SetComponentEnabled<BeeSeekingTag>(sortKey, bee.entity, true);
                ECB.SetComponentEnabled<BeeCarryingTag>(sortKey, bee.entity, false);
            }
        }
    }
}
