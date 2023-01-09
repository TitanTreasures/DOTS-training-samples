using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private float dropSpeed => _resourceComponent.ValueRO.droppingSpeed;

    public float3 GetResourcePosition()
    {
        return _transformAspect.LocalPosition;
    }
    public void FollowTarget()
    {
        float3 entityPos = _resourceComponent.ValueRO.currentBeeHolderPosition;
        _transformAspect.LocalPosition = entityPos + new float3(0,-3,0);
    }

    public bool IsInBaseLocationRange()
    {
        if (_transformAspect.LocalPosition.x >= (20 - 4) || _transformAspect.LocalPosition.x <= (-20 + 4))
            return true;
        return false;
    }

    public bool HasHitGround()
    {
        if (_transformAspect.LocalPosition.y <= 0)
            return true;
        return false;
    }

    public void DroppingMovement(float dt)
    {
        _transformAspect.LocalPosition += new float3(0, -1, 0) * dt * _resourceComponent.ValueRO.droppingSpeed;
    }
}
