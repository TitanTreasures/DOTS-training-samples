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

        DynamicBuffer<ResourcePositionElementBuffer> resourcePositionBuffer = state.EntityManager.GetBuffer<ResourcePositionElementBuffer>(bufferEntity);

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

                    resourcePositionBuffer = state.EntityManager.GetBuffer<ResourcePositionElementBuffer>(bufferEntity);
                    if (resourcePositionBuffer.Length != 0)
                    {
                        var randomBufferIndex = random.NextInt(resourcePositionBuffer.Length);
                        ecb.SetComponent<BeeTargetPositionComponent>(entity, new BeeTargetPositionComponent { targetPosition = resourcePositionBuffer.ElementAt(randomBufferIndex).Pos });
                        ecb.SetComponentEnabled(entity, typeof(BeeSeekingTag), true);
                    } else
                    {
                        ecb.SetComponentEnabled(entity, typeof(BeeIdleTag), true);
                    }
                    break;
            }
            ecb.SetComponentEnabled(entity, typeof(BeeIdleTag), false);
        }

        ecb.Playback(state.EntityManager);
    }
}
