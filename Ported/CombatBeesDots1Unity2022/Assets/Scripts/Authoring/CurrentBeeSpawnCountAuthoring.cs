using Unity.Entities;
using UnityEngine;

public class CurrentBeeSpawnCountAuthoring : MonoBehaviour
{
    public float value;
}

public class CurrentBeeSpawnCountBaker : Baker<CurrentBeeSpawnCountAuthoring>
{
    public override void Bake(CurrentBeeSpawnCountAuthoring authoring)
    {
        AddComponent(new CurrentBeeSpawnCount
        {
            value = authoring.value
        });
    }
}