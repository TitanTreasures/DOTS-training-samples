using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static ResourcePositionBufferSystem;
using static UnityEngine.EventSystems.EventTrigger;

public partial struct BeeBehaviourSystem : ISystem
{
    public Unity.Mathematics.Random random;

    EntityQuery idleQuery;

    //private BufferLookup<ResourcePositionElementBuffer> _bufferLookup;

    //EntityQuery attackingQuery;
    //EntityQuery seekingQuery;
    public void OnCreate(ref SystemState state)
    {
        random = Unity.Mathematics.Random.CreateFromIndex(1);
        idleQuery = state.GetEntityQuery(ComponentType.ReadOnly<BeeIdleTag>());
        //attackingQuery = state.GetEntityQuery(ComponentType.ReadOnly<BeeAttackingTag>());
        //seekingQuery = state.GetEntityQuery(ComponentType.ReadOnly<BeeSeekingTag>());

        //_bufferLookup = state.GetBufferLookup<ResourcePositionElementBuffer>(true);
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var amountOfBeeStates = 2;

        var bufferEntity = SystemAPI.GetSingletonEntity<ResourcePosBufferTag>();

        DynamicBuffer<ResourcePositionElementBuffer> myBuff = state.EntityManager.GetBuffer<ResourcePositionElementBuffer>(bufferEntity);

        foreach (var (tag, entity) in SystemAPI.Query<BeeIdleTag>().WithEntityAccess())
        {
            var randomBeeStateIndex = random.NextInt(amountOfBeeStates);
            
            switch (randomBeeStateIndex)
            {
                case 0:
                    // This is not necessary, but is included to help with testing
                    ecb.SetComponentEnabled(entity, typeof(BeeSeekingTag), false);

                    ecb.SetComponentEnabled(entity, typeof(BeeAttackingTag), false);
                    break;
                case 1:
                    // This is not necessary, but is included to help with testing
                    ecb.SetComponentEnabled(entity, typeof(BeeAttackingTag), false);

                    myBuff = state.EntityManager.GetBuffer<ResourcePositionElementBuffer>(bufferEntity);
                    ecb.SetComponent<BeeTargetPositionComponent>(entity, new BeeTargetPositionComponent { targetPosition = myBuff.ElementAt(random.NextInt(myBuff.Length)).Pos });
                    ecb.SetComponentEnabled(entity, typeof(BeeSeekingTag), true);
                    break;
            }
            ecb.SetComponentEnabled(entity, typeof(BeeIdleTag), false);
        }

        ecb.Playback(state.EntityManager);

        //var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        //new AssignBeeActivityJob
        //{
        //    resourceBuffer = resourceBuffer,
        //    ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
        //    AmountOfBeeStates = 2
        //}.ScheduleParallel(idleQuery);
    }

    //public partial struct AssignBeeActivityJob : IJobEntity
    //{
    //    public DynamicBuffer<ResourcePositionElementBuffer> resourceBuffer;
    //    public EntityCommandBuffer.ParallelWriter ECB;
    //    public int AmountOfBeeStates;

    //    private void Execute(BeeAspect bee, [EntityIndexInQuery] int sortKey)
    //    {
    //        var randomBeeStateIndex = bee.GetRandomBeeState(AmountOfBeeStates);

    //        switch (randomBeeStateIndex)
    //        {
    //            case 0:
    //                // This is not necessary, but is included to help with testing
    //                ECB.SetComponentEnabled<BeeSeekingTag>(sortKey, bee.entity, false);

    //                ECB.SetComponentEnabled<BeeAttackingTag>(sortKey, bee.entity, true);
    //                break;
    //            case 1:
    //                // This is not necessary, but is included to help with testing
    //                ECB.SetComponentEnabled<BeeAttackingTag>(sortKey, bee.entity, false);

    //                ECB.SetComponent<BeeTargetPositionComponent>(sortKey, bee.entity, new BeeTargetPositionComponent { targetPosition = resourceBuffer.ElementAt(bee.GetRandomResourceIndex(resourceBuffer.Length)).Pos });
    //                ECB.SetComponentEnabled<BeeSeekingTag>(sortKey, bee.entity, true);
    //                break;
    //        }
    //        ECB.SetComponentEnabled<BeeIdleTag>(sortKey, bee.entity, false);
    //    }
    //}
}
