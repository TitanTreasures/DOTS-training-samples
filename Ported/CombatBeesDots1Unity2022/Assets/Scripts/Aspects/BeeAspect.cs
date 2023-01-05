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
    private readonly RefRO<BeeSpawnLocationComponent> _spawnLocationComponent;

    private readonly RefRW<RandomComponent> _randomComponent;
    private readonly RefRW<WaitTimerComponent> _waitTimerComponent;


    private float3 attackTarget => _beePropertiesComponent.ValueRO.enemyTargetPosition;
    private float flySpeed => _beePropertiesComponent.ValueRO.flySpeed;
    private float3 targetPosition => _targetPositionComponent.ValueRO.targetPosition;
    private float3 basePosition => _spawnLocationComponent.ValueRO.basePosition;
    public float resourceInteractionRange => _beePropertiesComponent.ValueRO.resourceInteractionRange;
    public float attackRange => _beePropertiesComponent.ValueRO.attackRadius;


    // Timer variables:
    public float waitTimer => _waitTimerComponent.ValueRO.timer;
    public float maxWaitTime => _waitTimerComponent.ValueRW.maxWaitTime;

    public void MoveToAttackTarget(float deltaTime)
    {
        //Debug.Log(targetResourcePosition);
        float3 direction = math.normalize(attackTarget - _transformAspect.LocalPosition);
        //Debug.Log("Bee Position" + _transformAspect.Position);
        _transformAspect.LocalPosition += direction * deltaTime * (flySpeed * 2);
    }
    public void MoveToBase(float deltaTime)
    {
        //Debug.Log(targetResourcePosition);
        float3 direction = math.normalize(basePosition - _transformAspect.LocalPosition);
        //Debug.Log("Bee Position" + _transformAspect.Position);
        _transformAspect.LocalPosition += direction * deltaTime * flySpeed;
    }

    public void MoveTo(float deltaTime)
    {
        //Debug.Log(targetResourcePosition);
        float3 direction = math.normalize(targetPosition - _transformAspect.LocalPosition);
        //Debug.Log("Bee Position" + _transformAspect.Position);
        _transformAspect.LocalPosition += direction * deltaTime * flySpeed;
    }
    public bool TargetIsInAttackRange(float3 target)
    {
        if (math.distancesq(target, _transformAspect.LocalPosition) < attackRange)
        {
            return true;
        } else
        {
            return false;
        }
    }
    public bool IsInPickupRange()
    {
        return math.distancesq(targetPosition, _transformAspect.LocalPosition) <= resourceInteractionRange;
    }
    public bool IsInSpawnLocationRange()
    {
        return math.distancesq(basePosition, _transformAspect.LocalPosition) <= resourceInteractionRange;
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

    // Methods for updating the bees internal time
    public void UpdateWaitTimer(float deltaTime)
    {
        _waitTimerComponent.ValueRW.timer += deltaTime;
    }

    public bool CheckMaxTimer()
    {
        if (maxWaitTime < waitTimer)
        {
            ResetTimerAndSetNewRandomMaxWaitTime();
            return true;
        }
        return false;
    }
    private void ResetTimerAndSetNewRandomMaxWaitTime() 
    {
        _waitTimerComponent.ValueRW.timer = 0.0f;
        _waitTimerComponent.ValueRW.maxWaitTime = _randomComponent.ValueRW.randomValue.NextInt(10);
    }
}
