using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ResourceMono : MonoBehaviour
{
    public Entity holder;

    public Entity targetResource;
}
public class ResourceBaker : Baker<ResourceMono>
{
    public override void Bake(ResourceMono authoring)
    {
        AddComponent(new ResourceTag());

        AddComponent(new ResourcePropertiesComponent
        {
            currentBeeHolder = authoring.holder
        });

        AddComponent(new ResourceReadyForPickUpTag());
        //AddComponent(new TargetResourceComponent
        //{
        //    targetResource = authoring.targetResource
        //});
    }
}
