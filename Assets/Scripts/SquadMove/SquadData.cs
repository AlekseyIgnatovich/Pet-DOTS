using Unity.Entities;

// Отвечает за поведение отряда
public struct SquadData : IComponentData
{
    public float MoveSpeed;
    public float RotationSpeed;
    public int StartUnitsCount;
    public int RowCount;
}
