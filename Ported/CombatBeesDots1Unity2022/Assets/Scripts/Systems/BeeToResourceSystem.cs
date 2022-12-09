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
        //for testing
        // state.Enabled = true;

        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var resource in SystemAPI.Query<ResourceAspect>().WithAll<TargetResourceComponent>())
        {
            var randPos = resource.GetResourcePosition();

            //var rand = random.NextInt(0, resourcePositions.Count() - 1);
            //var randPos = resourcePositions[rand];
            new BeeToResourceJob
            {
                ResourcePos = randPos,
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

        
        private void Execute(BeeMoveToResourceAspect bee)
        {
            bee.FlyToResource(DeltaTime, ResourcePos);
        }
    }
}
