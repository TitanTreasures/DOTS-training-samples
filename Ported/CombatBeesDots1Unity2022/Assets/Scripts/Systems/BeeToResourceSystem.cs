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
using static UnityEngine.EventSystems.EventTrigger;

[BurstCompile]
public partial struct BeeToResourceSystem : ISystem
{
    public Unity.Mathematics.Random random;

    EntityQuery seekingQuery;
    EntityQuery carryingQuery;
    EntityQuery attackingQuery;


    DynamicBuffer<MyBufferElement> buffer;
    Entity e;
    EntityQuery resourceQuery;

    public struct MyBufferElement : IBufferElementData
    {
        // These implicit conversions are optional, but can help reduce typing.
        //public static implicit operator int(MyBufferElement e) { return e.Value; }
        //public static implicit operator MyBufferElement(int e) { return new MyBufferElement { Value = e }; }

        // Actual value each buffer element will store.
        public float3 Pos;
    }

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = Unity.Mathematics.Random.CreateFromIndex(1);
        seekingQuery = state.GetEntityQuery(ComponentType.ReadOnly<BeeSeekingTag>());
        carryingQuery = state.GetEntityQuery(ComponentType.ReadOnly<BeeCarryingTag>());
        attackingQuery = state.GetEntityQuery(ComponentType.ReadOnly<BeeAttackingTag>());

        resourceQuery = state.GetEntityQuery(ComponentType.ReadOnly<TargetResourceComponent>());


        e = state.EntityManager.CreateEntity(typeof(MyBufferElement));
        //state.EntityManager.AddBuffer<MyBufferElement>(e);
        

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // query for e in other systems
        buffer = state.EntityManager.GetBuffer<MyBufferElement>(e);
        foreach (var resourceAspect in SystemAPI.Query<TransformAspect>().WithAll<ResourceTag>())
        {
            Debug.Log(resourceAspect.LocalPosition);
            var element = new MyBufferElement
            {
                Pos = resourceAspect.LocalPosition
            };
            buffer.Append(element);
        }
        Debug.Log(buffer.Length);

        Debug.Log("First element:" + buffer[0]);

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
