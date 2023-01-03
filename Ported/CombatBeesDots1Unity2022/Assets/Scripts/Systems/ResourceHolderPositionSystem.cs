using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial struct ResourceHolderPositionSystem : ISystem
{
    EntityQuery beesCurrentlyHoldingQuuery;
    EntityQuery resourcesCurrentlyBeingHeldQuery;
    public void OnCreate(ref SystemState state)
    {
        beesCurrentlyHoldingQuuery = state.GetEntityQuery(ComponentType.ReadOnly<BeeCarryingTag>());
        resourcesCurrentlyBeingHeldQuery = state.GetEntityQuery(ComponentType.ReadOnly<ResourceBeingCarriedTag>());
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);


        var resources = 0;
        var bees = 0;

        foreach (var (resourceTransformAspect, resourceEntity) in SystemAPI.Query<TransformAspect>().WithAll<ResourceBeingCarriedTag>().WithEntityAccess())
        {
            resources++;
            float4 closestBee;
            closestBee = new float4(1000.0f, 1000.0f, 1000.0f, 1000.0f);
            bees = 0;

            foreach (var (beeTransformAspect, beeEntity) in SystemAPI.Query<TransformAspect>().WithAll<BeeCarryingTag>().WithEntityAccess())
            {
                bees++;
                var currentDistance = math.distancesq(resourceTransformAspect.WorldPosition, beeTransformAspect.WorldPosition);

                if (currentDistance < closestBee.w)
                {
                    closestBee = new float4(beeTransformAspect.WorldPosition.x,
                        beeTransformAspect.WorldPosition.y,
                        beeTransformAspect.WorldPosition.z,
                        currentDistance);

                }
            }
            Debug.Log(closestBee.xyz);
            ecb.SetComponent<ResourcePropertiesComponent>(resourceEntity, new ResourcePropertiesComponent { currentBeeHolderPosition = closestBee.xyz });
            
        }
        ecb.Playback(state.EntityManager);

        
    }
}
