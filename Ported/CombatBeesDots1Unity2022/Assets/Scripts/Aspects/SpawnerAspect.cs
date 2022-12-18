using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct SpawnerAspect : IAspect
{
    // For referencing the related entity
    public readonly Entity entity;
    // Spawner components for entity amount management
    private readonly RefRO<SpawnerComponent> _spawnerComponent;
    // Random component for positioning
    private readonly RefRW<RandomComponent> _randomComponent;

    // The values fetched from components and prefabs used in spawning
    public int resourceSpawnCount => _spawnerComponent.ValueRO.resourceSpawnCount;
    public int blueBeeSpawnCount => _spawnerComponent.ValueRO.blueBeeSpawnCount;
    public int yellowBeeSpawnCount => _spawnerComponent.ValueRO.yellowBeeSpawnCount;
    public Entity resourceSpawnPrefab => _spawnerComponent.ValueRO.resourcePrefab;
    public Entity blueBeeSpawnPrefab => _spawnerComponent.ValueRO.blueBeePrefab;
    public Entity yellowBeeSpawnPrefab => _spawnerComponent.ValueRO.yellowBeePrefab;

    public LocalTransform GetSpawnTransform(Entity entity)
    {
        LocalTransform transform = new LocalTransform();
        if (entity == resourceSpawnPrefab)
        {
            transform = GetSpawnTransform(_spawnerComponent.ValueRO.resourceFieldPosition,
                _spawnerComponent.ValueRO.resourceFieldDimensions);
        }else if (entity == blueBeeSpawnPrefab)
        {
            transform = GetSpawnTransform(_spawnerComponent.ValueRO.blueBeeFieldPosition,
                _spawnerComponent.ValueRO.blueBeeFieldDimensions);
        }
        else if (entity == yellowBeeSpawnPrefab)
        {
            transform = GetSpawnTransform(_spawnerComponent.ValueRO.yellowBeeFieldPosition,
                _spawnerComponent.ValueRO.yellowBeeFieldDimensions);
        }
        return transform;
    }

    private LocalTransform GetSpawnTransform(float3 fieldPosition, float3 fieldDimensions)
    {
        return new LocalTransform
        {
            Position = GetRandomPosition(fieldPosition, fieldDimensions),
            Rotation = quaternion.identity,
            Scale = 1f
        };
    }

    // Generate random spawn position for bee
    private float3 GetRandomPosition(float3 position, float3 field)
    {
        return _randomComponent.ValueRW.randomValue.NextFloat3(minCorner(position, field), maxCorner(position, field));
    }

    private float3 minCorner(float3 position, float3 field) => position - (field * 0.5f);
    private float3 maxCorner(float3 position, float3 field) => position + (field * 0.5f);
}
