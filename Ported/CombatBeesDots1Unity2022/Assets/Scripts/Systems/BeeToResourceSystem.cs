using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct BeeToResourceSystem : ISystem
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
        //for testing
        //state.Enabled = false;

        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        new BeeToResourceJob
        {
            DeltaTime = deltaTime,
            ResourcePos = float3.zero,
            ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel();
    }

    [BurstCompile]
    public partial struct BeeToResourceJob : IJobEntity
    {
        public float DeltaTime;
        public float3 ResourcePos;
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(BeeMoveToResourceAspect bee, [EntityInQueryIndex] int sortKey)
        {
            bee.FlyToResource(DeltaTime, ResourcePos);
            if (bee.IsInPickupRange(ResourcePos, 1f))
            {
                ECB.SetComponentEnabled<BeePropertiesComponent>(sortKey, bee.entity, false);
            }
        }
    }
}
