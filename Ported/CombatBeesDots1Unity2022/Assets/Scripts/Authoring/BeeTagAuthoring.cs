using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BeeTagAuthoring : MonoBehaviour
{

}

public class BeeTagBaker : Baker<BeeTagAuthoring>
{
    public override void Bake(BeeTagAuthoring authoring)
    {
        AddComponent(new BeeTag());
    }
}