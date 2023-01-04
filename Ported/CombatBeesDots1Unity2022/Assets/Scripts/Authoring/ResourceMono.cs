using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ResourceMono : MonoBehaviour
{
    public float3 targetPosition;
    public float droppingSpeed = 5f;
}
public class ResourceBaker : Baker<ResourceMono>
{
    public override void Bake(ResourceMono authoring)
    {
        AddComponent(new ResourcePropertiesComponent()
        {
            droppingSpeed = authoring.droppingSpeed
        });

        AddComponent(new BeeTargetPositionComponent
        {
            targetPosition = authoring.targetPosition
        });
    }
}
