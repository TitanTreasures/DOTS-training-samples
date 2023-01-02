using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public partial struct BeeBehaviourSystem : ISystem
{
    public Unity.Mathematics.Random random;

    EntityQuery idleQuery;
    EntityQuery attackingQuery;
    EntityQuery seekingQuery;
    public void OnCreate(ref SystemState state)
    {
        random = Unity.Mathematics.Random.CreateFromIndex(1);
        idleQuery = state.GetEntityQuery(ComponentType.ReadOnly<BeeIdleTag>());
        attackingQuery = state.GetEntityQuery(ComponentType.ReadOnly<BeeAttackingTag>());
        seekingQuery = state.GetEntityQuery(ComponentType.ReadOnly<BeeSeekingTag>());
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        //var i = 0;

        //foreach (var (tag, entity) in SystemAPI.Query<BeeIdleTag>().WithEntityAccess())
        //{
        //    i++;
        //    ecb.SetComponentEnabled(entity, typeof(BeeIdleTag), false);
        //}
        //Debug.Log("Entities" + i);

        //ecb.Playback(state.EntityManager);



        new AssignBeeActivityJob
        {
            ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            AmountOfBeeStates = 2
        }.ScheduleParallel(idleQuery);

    }

    public partial struct AssignBeeActivityJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        public int AmountOfBeeStates;

        private void Execute(BeeMoveAspect bee, [EntityIndexInQuery] int sortKey)
        {
            var randomBeeStateIndex = bee.GetRandomBeeState(AmountOfBeeStates);

            switch (randomBeeStateIndex)
            {
                case 0:
                    // This is not necessary, but is included to help with testing
                    ECB.SetComponentEnabled<BeeSeekingTag>(sortKey, bee.entity, false);

                    ECB.SetComponentEnabled<BeeAttackingTag>(sortKey, bee.entity, true);
                    break;
                case 1:
                    // This is not necessary, but is included to help with testing
                    ECB.SetComponentEnabled<BeeAttackingTag>(sortKey, bee.entity, false);

                    ECB.SetComponentEnabled<BeeSeekingTag>(sortKey, bee.entity, true);
                    break;
            }
            ECB.SetComponentEnabled<BeeIdleTag>(sortKey, bee.entity, false);
        }
    }
}
