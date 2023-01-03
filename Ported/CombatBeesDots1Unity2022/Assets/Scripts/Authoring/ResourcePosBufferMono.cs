using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ResourcePositionBufferMono : MonoBehaviour
{
}
public class ResourcePositionBufferBaker : Baker<ResourcePositionBufferMono>
{
    public override void Bake(ResourcePositionBufferMono authoring)
    {
        AddComponent(new ResourcePosBufferTag());
    }
}
