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
using static UnityEditor.Rendering.FilterWindow;
using static UnityEngine.EventSystems.EventTrigger;

[UpdateAfter(typeof(ResourcePositionBufferSystem))]
[UpdateBefore(typeof(MoveSystem))]
public partial struct ResourceHolderPositionSystem : ISystem
{
    

    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        EntityManager spa = state.EntityManager;

        float3 test = new float3(0.0f, 0.0f, 0.0f);

        var resources = 0;

        foreach (var (resourceTransformAspect, resourceEntity) in SystemAPI.Query<TransformAspect>().WithAll<ResourceBeingCarriedTag>().WithEntityAccess())
        {
            if (resourceTransformAspect.LocalPosition.x != test.x)
            {
                resources++;
                float3 closestBeePosition;
                closestBeePosition = new float3(1000.0f, 1000.0f, 1000.0f);

                float3 currentAdjustedResourcePos = new float3 (resourceTransformAspect.LocalPosition.x, resourceTransformAspect.LocalPosition.y + 3, resourceTransformAspect.LocalPosition.z);


                foreach (var (beeTransformAspect, beeEntity) in SystemAPI.Query<TransformAspect>().WithAll<BeeCarryingTag>().WithEntityAccess())
                {
                    var currentDistance = math.distancesq(currentAdjustedResourcePos, beeTransformAspect.LocalPosition);
                    var closestDistance = math.distancesq(currentAdjustedResourcePos, closestBeePosition);

                    if (currentDistance < closestDistance)
                    {
                        closestBeePosition = beeTransformAspect.LocalPosition;
                    }
                }
                var cool = spa.GetComponentData<ResourcePropertiesComponent>(resourceEntity);
                cool.currentBeeHolderPosition = closestBeePosition;
                spa.SetComponentData(resourceEntity, cool);
                
            }
        }
    }
}
