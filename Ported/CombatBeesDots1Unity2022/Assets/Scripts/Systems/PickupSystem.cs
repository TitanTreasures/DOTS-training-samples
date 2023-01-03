using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.PackageManager;
using UnityEngine;
using static MoveSystem;
using static UnityEditor.Rendering.FilterWindow;
using static UnityEngine.EventSystems.EventTrigger;

[BurstCompile]
public partial struct PickupSystem : ISystem
{
    public Unity.Mathematics.Random random;

    EntityQuery beeReadyToPickupQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = Unity.Mathematics.Random.CreateFromIndex(1);
        beeReadyToPickupQuery = state.GetEntityQuery(ComponentType.ReadOnly<BeeReadyToPickupTag>());
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

        var resourceBuffer = buffer;

        new BeePickupJob
        {
            DeltaTime = deltaTime,
            resourceBuffer = resourceBuffer,
            ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel(beeReadyToPickupQuery);
    }

    [BurstCompile]
    public partial struct BeePickupJob : IJobEntity
    {
        public float DeltaTime;
        public DynamicBuffer<ResourcePositionElementBuffer> resourceBuffer;
        public float3 targetPos;
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(BeeAspect bee, [EntityIndexInQuery] int sortKey)
        {
            float closestResourcePos = 99f;
            float3 closest = float3.zero;
            foreach(var element in resourceBuffer)
            {
                if (bee.GetDistanceToTarget(element.Pos) < closestResourcePos)
                    closest = element.Pos;
            }
            if (bee.GetDistanceToTarget(closest) > 2f)
            {
                ECB.SetComponentEnabled<BeeReadyToPickupTag>(sortKey, bee.entity, false);
                ECB.SetComponentEnabled<BeeIdleTag>(sortKey, bee.entity, true);
            }
        }
    }
}
