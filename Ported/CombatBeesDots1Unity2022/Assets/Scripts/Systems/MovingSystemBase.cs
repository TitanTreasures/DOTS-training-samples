using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class MovingSystemBase : SystemBase
{
    protected override void OnUpdate()
    {
        foreach ((TransformAspect transformAspect, RefRO<Speed> speed, RefRW<TargetPosition> targetPosition) in
            SystemAPI.Query<TransformAspect, RefRO<Speed>, RefRW<TargetPosition>>())
        {
            float3 direction = math.normalize(targetPosition.ValueRW.value - transformAspect.Position);
            transformAspect.Position += direction * SystemAPI.Time.DeltaTime * speed.ValueRO.value;
        }
    }
}
