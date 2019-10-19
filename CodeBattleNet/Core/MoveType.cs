namespace CodeBattleNet.Core
{
    public enum MoveType
    {
        Undefined,
        Random,
        MovementTo,
        MovementToClosestEnemy,
        PartialMovementToClosestEnemy,
        MovementToClosestConstruction,
        LongTermDirection,
        SafeMovement,
    }
}