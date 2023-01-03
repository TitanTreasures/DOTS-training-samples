using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ResourceMono : MonoBehaviour
{

}
public class ResourceBaker : Baker<ResourceMono>
{
    public override void Bake(ResourceMono authoring)
    {
        AddComponent(new ResourceTag());
        AddComponent(new ResourceBeingCarriedTag());
        AddComponent(new ResourcePropertiesComponent());
    }
}
