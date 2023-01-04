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

        foreach (var (resourceTransformAspect, resourceEntity) in SystemAPI.Query<TransformAspect>().WithAll<ResourceBeingCarriedTag>().WithEntityAccess())
        {
            resources++;
            float3 closestBeePosition;
            closestBeePosition = new float3(1000.0f, 1000.0f, 1000.0f);
            float3 adjustedResourcePosition = new float3(resourceTransformAspect.LocalPosition.x, resourceTransformAspect.LocalPosition.y + 3, resourceTransformAspect.LocalPosition.z);

            foreach (var (beeTransformAspect, beeEntity) in SystemAPI.Query<TransformAspect>().WithAll<BeeCarryingTag>().WithEntityAccess())
            {
                var currentDistance = math.distancesq(adjustedResourcePosition, beeTransformAspect.LocalPosition);
                var closestDistance = math.distancesq(adjustedResourcePosition, closestBeePosition);

                if (currentDistance < closestDistance)
                {
                    closestBeePosition = beeTransformAspect.LocalPosition;

                }
            }
            Debug.Log(closestBeePosition);
            ecb.SetComponent<ResourcePropertiesComponent>(resourceEntity, new ResourcePropertiesComponent { currentBeeHolderPosition = closestBeePosition });
            
        }
        ecb.Playback(state.EntityManager);

        
    }
}
