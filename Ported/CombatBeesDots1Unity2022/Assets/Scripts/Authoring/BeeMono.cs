using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BeeMono : MonoBehaviour
{
    public int team;
    public float flySpeed;
    public float resourceInteractionRange;
    public float attackRadius;
    public float3 targetPosition;
    public float3 enemyTargetPosition;

    // For randomness
    public uint randomSeed;
}
public class BeeBaker : Baker<BeeMono>
{
    public override void Bake(BeeMono authoring)
    {
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
            resourceInteractionRange = authoring.resourceInteractionRange * authoring.resourceInteractionRange,
            attackRadius = authoring.attackRadius * authoring.attackRadius 
        });
        AddComponent(new RandomComponent
        {
            randomValue = Unity.Mathematics.Random.CreateFromIndex(authoring.randomSeed)
        });
        AddComponent(new BeeTargetPositionComponent
        {
            targetPosition = authoring.targetPosition
        });
    }
}
