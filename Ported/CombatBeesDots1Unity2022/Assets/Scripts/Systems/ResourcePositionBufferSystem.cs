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

[UpdateBefore(typeof(MoveSystem))]
[UpdateBefore(typeof(BeeBehaviourSystem))]
[UpdateBefore(typeof(PickupSystem))]
public partial struct ResourcePositionBufferSystem : ISystem
{
    Entity e;

    public Unity.Mathematics.Random random;

    public struct ResourcePositionElementBuffer : IBufferElementData
    {
        public float3 Pos;
        public Entity Resource;
    }

    public void OnCreate(ref SystemState state)
    {
        random = Unity.Mathematics.Random.CreateFromIndex(1);
        e = state.EntityManager.CreateEntity(typeof(ResourcePositionElementBuffer));
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        e = SystemAPI.GetSingletonEntity<ResourcePosBufferTag>();
        var buffer = state.EntityManager.AddBuffer<ResourcePositionElementBuffer>(e);

        foreach (var (resourceTransformAspect, entity) in SystemAPI.Query<TransformAspect>().WithAll<ResourceDoesNotExistInBufferTag>().WithEntityAccess())
        {
            var element = new ResourcePositionElementBuffer
            {
                Pos = resourceTransformAspect.LocalPosition,
                Resource = entity 
            };
            buffer.Add(element);

            ecb.SetComponentEnabled(entity, typeof(ResourceDoesNotExistInBufferTag), false);
        }

        foreach (var (resourceTransformAspect, entity) in SystemAPI.Query<TransformAspect>().WithAll<ResourceBeingCarriedTag>().WithEntityAccess())
        {
            for(int i = 0; i < buffer.Length; i++) { 
                if(buffer.ElementAt(i).Resource.Equals(entity)) { 
                    buffer.RemoveAt(i); 
                }
            }
        }
        ecb.Playback(state.EntityManager);
    }
}
