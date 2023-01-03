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
        // These implicit conversions are optional, but can help reduce typing.
        //public static implicit operator int(MyBufferElement e) { return e.Value; }
        //public static implicit operator MyBufferElement(int e) { return new MyBufferElement { Value = e }; }

        // Actual value each buffer element will store.
        public float3 Pos;
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

        foreach (var (resourceTransformAspect, entity) in SystemAPI.Query<TransformAspect>().WithAll<ResourceReadyForPickUpTag>().WithEntityAccess())
        {
            //Debug.Log(resourceAspect.LocalPosition);
            var element = new ResourcePositionElementBuffer
            {
                Pos = resourceTransformAspect.LocalPosition
            };
            buffer.Add(element);

            ecb.SetComponentEnabled(entity, typeof(ResourceReadyForPickUpTag), false);
        }
        ecb.Playback(state.EntityManager);
    }
}
