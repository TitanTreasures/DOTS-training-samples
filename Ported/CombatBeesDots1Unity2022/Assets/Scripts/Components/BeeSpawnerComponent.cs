using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct BeeSpawnerComponent : IComponentData
{
    public Entity beePrefab;
    public float totalSpawnCount;
    public float currentSpawnCount;
}
