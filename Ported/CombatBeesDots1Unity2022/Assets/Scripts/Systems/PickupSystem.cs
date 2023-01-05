using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.PackageManager;
using UnityEditorInternal;
using UnityEngine;
using static ResourcePositionBufferSystem;
using static UnityEditor.Rendering.FilterWindow;
using static UnityEngine.EventSystems.EventTrigger;

[BurstCompile]
public partial struct PickupSystem : ISystem
{
    public Unity.Mathematics.Random random;

    EntityQuery beeReadyToPickupQuery;
    //EntityQuery resourceReadyForPickUpQuery;
    //EntityQuery combinedPickupQuery;

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
        return;
        //var deltaTime = SystemAPI.Time.DeltaTime;
        //var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();


        EntityManager spa = state.EntityManager;

        bool test = true;

        foreach (var (bee, beeEntity) in SystemAPI.Query<BeeAspect>().WithAll<BeeReadyToPickupTag>().WithEntityAccess())
        {
            foreach (var (resourceTransformAspect, resourceentity) in SystemAPI.Query<TransformAspect>().WithAll<ResourceReadyForPickUpTag>().WithEntityAccess())
            {
                if (bee.GetDistanceToTarget(resourceTransformAspect.WorldPosition) < bee.pickupRange * 2)
                {
                    spa.SetComponentEnabled<BeeReadyToPickupTag>(bee.entity, false);
                    spa.SetComponentEnabled<BeeCarryingTag>(bee.entity, true);
                    spa.SetComponentEnabled<ResourceReadyForPickUpTag>(resourceentity, false);
                    spa.SetComponentEnabled<ResourceBeingCarriedTag>(resourceentity, true);
                    test = false;
                }
            }
            if (test)
            {
                spa.SetComponentEnabled<BeeReadyToPickupTag>(bee.entity, false);
                spa.SetComponentEnabled<BeeIdleTag>(bee.entity, true);
            }
        }
        

        //new BeePickupJob
        //{
        //    DeltaTime = deltaTime,
        //    ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        //}.ScheduleParallel(beeReadyToPickupQuery);
    }

    //[BurstCompile]
    //public partial struct BeePickupJob : IJobEntity
    //{
    //    public float DeltaTime;
    //    public float3 targetPos;
    //    public EntityCommandBuffer.ParallelWriter ECB;

    //    [BurstCompile]
    //    private void Execute(BeeAspect bee, [EntityIndexInQuery] int sortKey)
    //    {
    //        bool test = true;
            
    //    }
    //}
}
