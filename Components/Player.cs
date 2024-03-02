using System.Numerics;
using GramEngine.ECS;

namespace CardCrawl.Components;

public class Player : Component
{
    public int DungeonX;
    public int DungeonY;
    public int Direction;

    public Player((int x, int y) position, int direction)
    {
        DungeonX = position.x;
        DungeonY = position.y;
        Direction = direction;

    }
}