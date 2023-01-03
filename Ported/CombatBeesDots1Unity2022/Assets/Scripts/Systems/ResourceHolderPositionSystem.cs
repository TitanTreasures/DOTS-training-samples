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
            float3 closestBeePosition;
            closestBeePosition = new float3(1000.0f, 1000.0f, 1000.0f);
            bees = 0;

            foreach (var (beeTransformAspect, beeEntity) in SystemAPI.Query<TransformAspect>().WithAll<BeeCarryingTag>().WithEntityAccess())
            {
                bees++;
                var currentDistance = math.distancesq(resourceTransformAspect.LocalPosition, beeTransformAspect.LocalPosition);
                //Debug.Log("Current bee dis: " + currentDistance);
                var closestDistance = math.distancesq(resourceTransformAspect.LocalPosition, closestBeePosition);
                //Debug.Log("Closest bee dis: " + closestDistance);

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
