using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct ResourceAspect : IAspect
{
    // For referencing the related resource entity
    public readonly Entity entity;
    // Transform needed to manipulate resource position
    private readonly TransformAspect _transformAspect;
    // TargetResourceComponent added to hopefully only have the aspect apply to resources
    //private readonly RefRO<TargetResourceComponent> _targetResourceComponent;
    private readonly RefRO<ResourcePropertiesComponent> _resourceComponent;

    public float3 GetResourcePosition()
    {
        return _transformAspect.LocalPosition;
        //float3 direction = math.normalize( targetPosition.ValueRW.value - transformAspect.Position);
        //_transformAspect.Position += direction * deltaTime * speed.ValueRO.value;
    }
    public void FollowTarget()
    {
        float3 entityPos = _resourceComponent.ValueRO.currentBeeHolderPosition;
        _transformAspect.LocalPosition = entityPos + new float3(0,-1,0);
        
    }
}
