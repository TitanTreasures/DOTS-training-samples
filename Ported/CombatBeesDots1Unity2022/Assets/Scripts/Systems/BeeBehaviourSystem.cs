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

[BurstCompile]
public partial struct BeeBehaviourSystem : ISystem
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
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var amountOfBeeStates = 1;

        var bufferEntity = SystemAPI.GetSingletonEntity<ResourcePosBufferTag>();

        DynamicBuffer<ResourcePositionElementBuffer> resourcePositionBuffer = state.EntityManager.GetBuffer<ResourcePositionElementBuffer>(bufferEntity);

        if (resourcePositionBuffer.Length != 0)
        {
            foreach (var (tag, entity) in SystemAPI.Query<BeeIdleTag>().WithEntityAccess())
            {
                var randomBeeStateIndex = random.NextInt(amountOfBeeStates);

                switch (randomBeeStateIndex)
                {
                    case 0:
                        // This is not necessary, but is included to help with testing
                        ecb.SetComponentEnabled(entity, ComponentType.ReadOnly<BeeAttackingTag>(), false);

                        resourcePositionBuffer = state.EntityManager.GetBuffer<ResourcePositionElementBuffer>(bufferEntity);
                        if (resourcePositionBuffer.Length != 0)
                        {
                            var randomBufferIndex = random.NextInt(resourcePositionBuffer.Length);
                            ecb.SetComponent<BeeTargetPositionComponent>(entity, new BeeTargetPositionComponent { targetPosition = resourcePositionBuffer.ElementAt(randomBufferIndex).Pos });
                            ecb.SetComponentEnabled(entity, ComponentType.ReadOnly<BeeSeekingTag>(), true);
                            ecb.SetComponentEnabled(entity, ComponentType.ReadOnly<BeeIdleTag>(), false);
                        }
                        else
                        {
                            ecb.SetComponentEnabled(entity, ComponentType.ReadOnly<BeeIdleTag>(), true);
                        }
                        break;
                    // This code is left in comment form as the BeeAttacking functionality had to be abandoned due to time limitations
                    // despite it being nearly functional. It is left in comment form for context.
                    //case 1:
                    //    // This is not necessary, but is included to help with testing
                    //    ecb.SetComponentEnabled(entity, ComponentType.ReadOnly<BeeSeekingTag>(), false);

                    //    ecb.SetComponentEnabled(entity, typeof(BeeAttackingTag), false);
                    //    ecb.SetComponentEnabled(entity, typeof(BeeIdleTag), true);
                    //    break;
                }
            }
        }
        ecb.Playback(state.EntityManager);
    }
}
