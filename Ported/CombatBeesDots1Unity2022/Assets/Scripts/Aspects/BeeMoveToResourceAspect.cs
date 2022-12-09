using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct BeeMoveToResourceAspect : IAspect
{
    private readonly Entity entity;

    private readonly TransformAspect _transformAspect;
    private readonly RefRO<BeePropertiesComponent> _beePropertiesComponent;

    private readonly RefRW<RandomComponent> _randomComponent;

    // Inside the job that will eventually work with this, remove resource component when done
    // to ensure the system is not acting on that bee entity anymore

    private float flySpeed => _beePropertiesComponent.ValueRO.flySpeed;
    //private Entity targetResource => _targetResourceComponent.ValueRO.targetResource;

    public void FlyToResource(float deltaTime, float3 targetResourcePosition)
    {
        //Debug.Log(targetResourcePosition);
        float3 direction = math.normalize(targetResourcePosition - _transformAspect.Position);
        //Debug.Log("Bee Position" + _transformAspect.Position);
        _transformAspect.Position += direction * deltaTime * flySpeed;
    }

    public int GetRandomResourceIndex(int resourcesAmount)
    {
        return _randomComponent.ValueRW.randomValue.NextInt(resourcesAmount);
    }
}
