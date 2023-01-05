using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
public partial struct ActionDurationTimerSystem : ISystem
{
    public Unity.Mathematics.Random random;

    Entity e;

    EntityQuery waitingBeesQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = Unity.Mathematics.Random.CreateFromIndex(1);
        waitingBeesQuery = state.GetEntityQuery(ComponentType.ReadOnly<WaitTimerComponent>());
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

        new updateActionDurationTimerJob
        {
            DeltaTime = deltaTime,
            ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel(waitingBeesQuery);
    }

    [BurstCompile]
    public partial struct updateActionDurationTimerJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(BeeAspect bee, [EntityIndexInQuery] int sortKey)
        {
            bee.UpdateWaitTimer(DeltaTime);
            if (bee.CheckMaxTimer())
            {
                ECB.SetComponentEnabled<BeeAttackingTag>(sortKey, bee.entity, false);
                ECB.SetComponentEnabled<BeeSeekingTag>(sortKey, bee.entity, false);
                ECB.SetComponentEnabled<WaitTimerComponent>(sortKey, bee.entity, false);
                ECB.SetComponentEnabled<BeeIdleTag>(sortKey, bee.entity, true);
            }
        }
    }
}
