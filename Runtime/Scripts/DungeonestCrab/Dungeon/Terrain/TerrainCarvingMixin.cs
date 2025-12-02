using DungeonestCrab.Dungeon;
using Sirenix.OdinInspector;
using System;

[Serializable]
public class TerrainCarvingMixin : TerrainMixin {
    [Title("A* Costs")]
    public float WallCarveCost = 6;
    public float FloorCarveCost = 1;
}