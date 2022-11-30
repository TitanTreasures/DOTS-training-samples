using Unity.Entities;
using UnityEngine;

public class MaxSpawnCountAuthoring : MonoBehaviour
{
    public float value;
}

public class MaxSpawnCountBaker : Baker<MaxSpawnCountAuthoring>
{
    public override void Bake(MaxSpawnCountAuthoring authoring)
    {
        AddComponent(new MaxSpawnCount
        {
            value = authoring.value
        });
    }
}