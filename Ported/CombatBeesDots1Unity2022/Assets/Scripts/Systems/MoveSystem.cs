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
public partial struct MoveSystem : ISystem
{
    public Unity.Mathematics.Random random;

    EntityQuery seekingQuery;
    EntityQuery carryingQuery;
    EntityQuery resourceBeingCarriedQuery;
    EntityQuery attackingQuery;
    EntityQuery resourceFollowQuery;


    public static DynamicBuffer<ResourcePositionElementBuffer> buffer;
    Entity e;
    EntityQuery resourceQuery;

    public struct ResourcePositionElementBuffer : IBufferElementData
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
        resourceBeingCarriedQuery = state.GetEntityQuery(ComponentType.ReadOnly<ResourceBeingCarriedTag>());
        resourceQuery = state.GetEntityQuery(ComponentType.ReadOnly<TargetResourceComponent>());

        e = state.EntityManager.CreateEntity(typeof(ResourcePositionElementBuffer));

        
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // TODO: THIS BUFFER IS ADDING RESOURCES INDEFINATELY
        // query for e in other systems
        buffer = state.EntityManager.GetBuffer<ResourcePositionElementBuffer>(e);
        if (buffer.Length > 0)
        {
            //Debug.Log(buffer.Length);
        }
        foreach (var resourceAspect in SystemAPI.Query<TransformAspect>().WithAll<ResourceTag>())
        {
            //Debug.Log(resourceAspect.LocalPosition);
            var element = new ResourcePositionElementBuffer
            {
                Pos = resourceAspect.LocalPosition
            };
            buffer.Add(element);
        }

        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        new BeeSeekingJob
        {
            DeltaTime = deltaTime,
            ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel(seekingQuery);

        new BeeCarryingJob
        {
            DeltaTime = deltaTime,
            ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel(carryingQuery);

        new ResourceFollowJob
        {
            DeltaTime = deltaTime,
            ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel(resourceBeingCarriedQuery);
    }

    [BurstCompile]
    public partial struct BeeSeekingJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(BeeAspect bee, [EntityIndexInQuery] int sortKey)
        {
            bee.MoveTo(DeltaTime, float3.zero);
            if (bee.IsInPickupRange(float3.zero, 1f))
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
            bee.MoveTo(DeltaTime, BasePos);
            if (bee.IsInPickupRange(BasePos, 1f))
            {
                ECB.SetComponentEnabled<BeeSeekingTag>(sortKey, bee.entity, true);
                ECB.SetComponentEnabled<BeeCarryingTag>(sortKey, bee.entity, false);
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
        }
    }
}
