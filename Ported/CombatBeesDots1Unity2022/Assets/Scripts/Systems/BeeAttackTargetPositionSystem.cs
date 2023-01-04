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

[UpdateBefore(typeof(MoveSystem))]
public partial struct BeeAttackTargetPositionSystem : ISystem
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

        foreach (var (blueTransformAspect, blueBeeEntity) in SystemAPI.Query<BeeAspect>().WithAll<BeeBlueTag>().WithAll<BeeAttackingTag>().WithEntityAccess())
        {
            float3 firstFoundEnemyBeePosition;
            firstFoundEnemyBeePosition = new float3(100.0f, 100.0f, 100.0f);

            foreach (var (yellowTransformAspect, beeEntity) in SystemAPI.Query<TransformAspect>().WithAll<BeeYellowTag>().WithEntityAccess())
            {
                if(blueTransformAspect.TargetIsInAttackRange(yellowTransformAspect.WorldPosition))
                {
                    firstFoundEnemyBeePosition = yellowTransformAspect.WorldPosition;
                    Debug.Log(yellowTransformAspect.WorldPosition);
                    break;
                }
            }

            // No enemy bees were in range, so go back to idle and try again.
            if(firstFoundEnemyBeePosition.x == 100.0f)
            {
                spa.SetComponentEnabled(blueBeeEntity, typeof(BeeAttackingTag), false);
                spa.SetComponentEnabled(blueBeeEntity, typeof(BeeIdleTag), true);
            } else
            {
                var cool = spa.GetComponentData<BeePropertiesComponent>(blueBeeEntity);
                cool.enemyTargetPosition = firstFoundEnemyBeePosition;
                spa.SetComponentData(blueBeeEntity, cool);
            }
        }
    }
}
