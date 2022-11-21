using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.Serialization.Json.JsonWriter;
using UnityEngine.XR;

public class SpawnManager : MonoBehaviour
{
    public bool useEntities;

    EntityManager entityManager;
    Entity prototype;
    EntityCommandBuffer ecbJob;

    public List<Mesh> meshes;
    public List<Material> materials;

    public float objectScale;

    public static SpawnManager instance;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        ecbJob = new EntityCommandBuffer(Allocator.TempJob);
    }
    public void SpawnBee(int team)
    {
        Vector3 pos = Vector3.right * (-Field.size.x * .4f + Field.size.x * .8f * team);
        _SpawnBeeAsEntity(pos, team);
    }

    void _SpawnBeeAsEntity(Vector3 pos, int team)
    {
        prototype = entityManager.CreateEntity();
        //manager.AddComponent(beeEntity, ComponentType.ChunkComponent<Transform>());
        entityManager.AddComponentData(prototype, new Translation { Value = pos });
        // Use set to set after init
        //manager.SetComponentData(beeEntity, new Translation { Value = pos });
      
        var spawnJob = new SpawnJob
        {
            Prototype = prototype,
            Material = materials[team],
            ObjectScale = objectScale,
            Ecb = ecbJob.AsParallelWriter()
        };
        var spawnHandle = spawnJob.Schedule(100, 128);
        spawnHandle.Complete();
        ecbJob.Playback(entityManager);
        ecbJob.Dispose();
        entityManager.DestroyEntity(prototype);
    }
    
    public struct SpawnJob : IJobParallelFor
    {
        public Entity Prototype;
        public Material Material;
        public float ObjectScale;
        public EntityCommandBuffer.ParallelWriter Ecb;

        [ReadOnly]
        public NativeArray<RenderBounds> MeshBounds;

        public void Execute(int index)
        {
            var e = Ecb.Instantiate(index, Prototype);
            // Prototype has all correct components up front, can use SetComponent
            Ecb.SetComponent(index, e, new LocalToWorld { Value = ComputeTransform(index) });
            Ecb.SetComponent(index, e, new MaterialColor() { Value = ComputeColor(index) });
            // MeshBounds must be set according to the actual mesh for culling to work.
            //int meshIndex = index % MeshCount;
            //Ecb.SetComponent(index, e, MaterialMeshInfo.FromRenderMeshArrayIndices(0, meshIndex));
            //Ecb.SetComponent(index, e, MeshBounds[meshIndex]);
        }

        public float4 ComputeColor(int index)
        {
            var color = Material.color;
            return new float4(color.r, color.g, color.b, 1);
        }

        public float4x4 ComputeTransform(int index)
        {

            float x = 0;
            float z = 0;
            float y = 0;

            float4x4 M = float4x4.TRS(
                new float3(x, y, z),
                quaternion.identity,
                new float3(ObjectScale));

            return M;
        }
    }
}
