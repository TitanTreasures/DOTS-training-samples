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
        //var ecb = new EntityCommandBuffer(Allocator.Temp);

        var resourcePositions = new List<float3>();
        foreach (var resource in SystemAPI.Query<ResourceAspect>().WithAll<TargetResourceComponent>())
        {
            resourcePositions.Add(resource.GetResourcePosition());
        }
        foreach (var bee in SystemAPI.Query<BeeMoveToResourceAspect>().WithAll<BeePropertiesComponent>())
        {
            var randomIndex = bee.GetRandomResourceIndex(resourcePositions.Count);
            new BeeToResourceJob
            {
                ResourcePos = resourcePositions[randomIndex],
                DeltaTime = deltaTime,
            }.Run();
        }
    }

    [BurstCompile]
    public partial struct BeeToResourceJob : IJobEntity
    {
        public float DeltaTime;
        public float3 ResourcePos;
        //public EntityCommandBuffer ECB;

        [BurstCompile]
        private void Execute(BeeMoveToResourceAspect bee)
        {
            bee.FlyToResource(DeltaTime, ResourcePos);
        }
    }
}
