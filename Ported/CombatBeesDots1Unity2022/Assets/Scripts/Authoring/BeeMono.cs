using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BeeMono : MonoBehaviour
{
    public int team;
    public float flySpeed;

    // For randomness
    public uint randomSeed;
}
public class BeeBaker : Baker<BeeMono>
{
    public override void Bake(BeeMono authoring)
    {
        AddComponent(new BeeSeekingTag());
        AddComponent(new BeeCarryingTag());
        AddComponent(new BeeAttackingTag());

        if (authoring.team == 0)
        {
            AddComponent(new BeeBlueTag());
        }
        else if(authoring.team == 1)
        {
            AddComponent(new BeeYellowTag());
        }
        AddComponent(new BeePropertiesComponent
        {
            flySpeed = authoring.flySpeed,
        });
        AddComponent(new RandomComponent
        {
            randomValue = Unity.Mathematics.Random.CreateFromIndex(authoring.randomSeed)
        });
    }
}
