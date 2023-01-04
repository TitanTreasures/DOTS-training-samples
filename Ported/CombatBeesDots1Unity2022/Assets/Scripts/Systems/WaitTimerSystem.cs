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
public partial struct WaitTimerSystem : ISystem
{
    public Unity.Mathematics.Random random;

    Entity e;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = Unity.Mathematics.Random.CreateFromIndex(1);
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

        new updateWaitTimerJob
        {
            DeltaTime = deltaTime,
            ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel(seekingQuery);
    }

    [BurstCompile]
    public partial struct updateWaitTimerJob : IJobEntity
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
        private void Execute(ResourceAspect resource, [EntityIndexInQuery] int sortKey)
        {
            resource.FollowTarget();
            if (resource.IsInBaseLocationRange())
            {
                ECB.SetComponentEnabled<ResourceBeingCarriedTag>(sortKey, resource.entity, false);
                ECB.SetComponentEnabled<ResourceDroppingTag>(sortKey, resource.entity, true);
            }
        }
    }
    [BurstCompile]
    public partial struct ResourceDroppingJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(ResourceAspect resource, [EntityIndexInQuery] int sortKey)
        {
            resource.DroppingMovement(DeltaTime);

            if (resource.HasHitGround())
            {
                if (resource.IsInBaseLocationRange())
                {
                    ECB.SetComponentEnabled<ResourceDroppingTag>(sortKey, resource.entity, false);
                    ECB.SetComponentEnabled<ResourceDespawnTag>(sortKey, resource.entity, true);
                }
                else
                {
                    ECB.SetComponentEnabled<ResourceDroppingTag>(sortKey, resource.entity, false);
                    ECB.SetComponentEnabled<ResourceReadyForPickUpTag>(sortKey, resource.entity, true);
                    ECB.SetComponentEnabled<ResourceDoesNotExistInBufferTag>(sortKey, resource.entity, true);
                }
            }
        }
    }
}
