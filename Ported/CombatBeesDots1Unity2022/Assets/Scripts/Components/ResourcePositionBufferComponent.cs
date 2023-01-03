using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct ResourcePositionBufferComponent : IBufferElementData
{
    public float3 Pos;
}