using Latios.Kinemation;
using Unity.Entities;

public struct SingleClip : IComponentData
{
    public BlobAssetReference<SkeletonClipSetBlob> blob;
}