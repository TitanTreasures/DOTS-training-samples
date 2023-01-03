using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct BeeAspect : IAspect
{
    public readonly Entity entity;

    public readonly TransformAspect _transformAspect;
    private readonly RefRO<BeePropertiesComponent> _beePropertiesComponent;
    private readonly RefRO<BeeTargetPositionComponent> _targetPositionComponent;

    private readonly RefRW<RandomComponent> _randomComponent;



    private float flySpeed => _beePropertiesComponent.ValueRO.flySpeed;
    private float3 targetPosition => _targetPositionComponent.ValueRO.targetPosition;
    //private Entity targetResource => _targetResourceComponent.ValueRO.targetResource;

    public void MoveTo(float deltaTime)
    {
        //Debug.Log(targetResourcePosition);
        float3 direction = math.normalize(targetPosition - _transformAspect.LocalPosition);
        //Debug.Log("Bee Position" + _transformAspect.Position);
        _transformAspect.LocalPosition += direction * deltaTime * flySpeed;
    }
    public bool IsInPickupRange(float3 resourcePos, float resourceRadiusSq)
    {
        return math.distancesq(resourcePos, _transformAspect.LocalPosition) <= resourceRadiusSq;
    }

    public float GetDistanceToTarget(float3 target)
    {
        return math.distancesq(target, _transformAspect.LocalPosition);
    }

    public int GetRandomResourceIndex(int resourcesAmount)
    {
        return _randomComponent.ValueRW.randomValue.NextInt(resourcesAmount);
    }

    public int GetRandomBeeState(int amountOfPossibleBeeStates)
    {
        return _randomComponent.ValueRW.randomValue.NextInt(amountOfPossibleBeeStates);
    }
}
