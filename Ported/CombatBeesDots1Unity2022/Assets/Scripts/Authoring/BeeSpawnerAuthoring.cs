using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class BeeSpawnerAuthoring : MonoBehaviour
{
    public GameObject beePrefab;
    public float totalSpawnCount;
    public float currentSpawnCount;
}

public class BeeSpawnerBaker : Baker<BeeSpawnerAuthoring>
{
    public override void Bake(BeeSpawnerAuthoring authoring)
    {
        AddComponent(new BeeSpawnerComponent
        {
            beePrefab = GetEntity(authoring.beePrefab),
            totalSpawnCount = authoring.totalSpawnCount,
            currentSpawnCount = authoring.currentSpawnCount
});
    }
}
