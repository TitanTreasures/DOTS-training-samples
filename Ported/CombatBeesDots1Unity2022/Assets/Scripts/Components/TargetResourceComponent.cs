using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct TargetResourceComponent : IComponentData, IEnableableComponent
{
    public Entity targetResource;
}

// This is used as a tag to apply logic to the TargetResourceComponent component later
public struct TargetResourceComponentTag : IComponentData { }
