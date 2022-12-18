using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct ResourceSpawnerAspect : IAspect
{
    // For referencing the related entity
    public readonly Entity entity;
    // Transform needed for aspect to apply to beeSpawner
    private readonly TransformAspect _transformAspect;
    // Spawner components for entity amount management
    private readonly RefRO<ResourceSpawnerComponent> _ResourceSpawnerComponent;
    // Random component for positioning
    private readonly RefRW<RandomComponent> _randomComponent;

    // The values fetched from components and prefabs used in spawning
    public int maxResourceSpawnCount => _ResourceSpawnerComponent.ValueRO.maxResourceSpawnCount;
    public Entity resourcePrefab => _ResourceSpawnerComponent.ValueRO.resourcePrefab;

    // Using ust here because it supposedly is an easy/performance friendly way to transform
    public UniformScaleTransform GetRandomResourceTransform()
    {
        return new UniformScaleTransform
        {
            Position = GetRandomPosition(),
            Rotation = quaternion.identity,
            Scale = 1f
        };
    }

    // Generate random spawn position for bee
    private float3 GetRandomPosition()
    {
        float3 randomPosition;
        randomPosition = _randomComponent.ValueRW.randomValue.NextFloat3(minCorner, maxCorner);
        return randomPosition;
    }

    private float3 minCorner => _transformAspect.Position - halfDimensions;
    private float3 maxCorner => _transformAspect.Position + halfDimensions;
    private float3 halfDimensions => new()
    {
        x = _ResourceSpawnerComponent.ValueRO.fieldDimensions.x * 0.5f,
        y = _ResourceSpawnerComponent.ValueRO.fieldDimensions.y * 0.5f,
        z = _ResourceSpawnerComponent.ValueRO.fieldDimensions.z * 0.5f
    };
}
