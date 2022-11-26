using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class RandomAuthoring: MonoBehaviour
{

}

public class RandomBaker : Baker<RandomAuthoring>
{
    public override void Bake(RandomAuthoring authoring)
    {
        AddComponent(new RandomComponent
        {
            random = new Unity.Mathematics.Random((uint)SystemAPI.Time.ElapsedTime)
        });
    }
}