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
        private void Execute(BeeMoveToResourceAspect bee, [EntityIndexInQuery] int sortKey)
        {
            bee.FlyToResource(DeltaTime, ResourcePos);
            if (bee.IsInPickupRange(ResourcePos, 1f))
            {
                ECB.SetComponentEnabled<BeePropertiesComponent>(sortKey, bee.entity, false);
            }
        }
    }
}
