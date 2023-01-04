using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct WaitTimerComponent : IComponentData, IEnableableComponent
{
    public float timer;
}
