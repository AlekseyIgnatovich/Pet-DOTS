using Latios.Kinemation;
using Latios.Transforms;
using Latios.Transforms.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

using static Unity.Entities.SystemAPI;

namespace Dragons
{
    [UpdateBefore(typeof(TransformSuperSystem))]
    public partial struct SingleClipPlayerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float t = (float)Time.ElapsedTime;

            foreach ((var bones, var singleClip) in Query<DynamicBuffer<BoneReference>, RefRO<SingleClip>>())
            {

                ref var clip = ref singleClip.ValueRO.blob.Value.clips[0];
                
                var clipTime = clip.LoopToClipTime(t);
                
                for (int i = 1; i < bones.Length; i++)
                {
                    var boneSampledLocalTransform          = clip.SampleBone(i, clipTime);
                    var boneTransformAspect                = GetAspect<TransformAspect>(bones[i].bone);
                    boneTransformAspect.localTransformQvvs = boneSampledLocalTransform;
                }
                
            }
        }
    }
}