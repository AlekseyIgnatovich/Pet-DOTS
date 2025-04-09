using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct FormationSystem : ISystem
{
    private int _tempCount;// ToDo: remove it

    public void OnCreate()
    {
        _tempCount = 0;
    }

    public void OnUpdate(ref SystemState state)
    {
        _tempCount++;
        if(_tempCount > 100)
            return;
        
        bool switchToWideFormation = true; // флаг: когда хочешь перестроиться

        int rowCount = switchToWideFormation ? 3 : 1;
        float spacing = 2f;
        float3 origin = new float3(0, 0, 0);

        foreach (var (unit, transform) in SystemAPI.Query<RefRO<FormationUnit>, RefRW<TargetPosition>>())
        {
            int index = unit.ValueRO.Index;

            int row = index / rowCount;
            int col = index % rowCount;

            float3 targetPos = origin + new float3(col * spacing, 0, row * spacing);
            transform.ValueRW.Value = targetPos;
        }
    }
}
