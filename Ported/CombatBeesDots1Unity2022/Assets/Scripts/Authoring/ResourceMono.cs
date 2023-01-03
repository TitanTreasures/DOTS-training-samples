using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ResourceMono : MonoBehaviour
{
    public float3 targetPosition;
}
public class ResourceBaker : Baker<ResourceMono>
{
    public override void Bake(ResourceMono authoring)
    {
        AddComponent(new ResourceTag());
        AddComponent(new ResourceBeingCarriedTag());
        AddComponent(new ResourcePropertiesComponent());

        AddComponent(new BeeTargetPositionComponent
        {
            targetPosition = authoring.targetPosition
        });
    }
}
