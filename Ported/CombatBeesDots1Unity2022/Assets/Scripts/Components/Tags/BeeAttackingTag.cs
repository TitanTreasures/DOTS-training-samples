using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct BeeAttackingTag : IComponentData, IEnableableComponent
{
    public Entity target;
}
