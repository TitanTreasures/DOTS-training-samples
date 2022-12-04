using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BeeMono : MonoBehaviour
{
    public int team;
}
public class BeeBaker : Baker<BeeMono>
{
    public override void Bake(BeeMono authoring)
    {
        AddComponent(new BeePropertiesComponent
        {
            team = authoring.team 
        });
    }
}
