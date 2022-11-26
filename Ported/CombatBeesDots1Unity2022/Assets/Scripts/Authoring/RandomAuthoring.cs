using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class RandomAuthoring: MonoBehaviour
{
    //uint seed = (uint)SystemAPI.Time.ElapsedTime;
}

public class RandomBaker : Baker<RandomAuthoring>
{
    public override void Bake(RandomAuthoring authoring)
    {
        AddComponent(new RandomComponent
        {
            random = new Unity.Mathematics.Random(1)
        });
    }
}