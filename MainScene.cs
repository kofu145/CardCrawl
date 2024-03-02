using System.Numerics;
using CardCrawl.Components;
using CardCrawl.Models;
using GramEngine.Core;
using GramEngine.ECS;
using GramEngine.ECS.Components;
using Newtonsoft.Json;

namespace CardCrawl;

public class MainScene : GameState
{
    public override void Initialize()
    {
        base.Initialize();
        int[][] mapData = [
            [1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,0,0,0,0,0,0,1,0,0,0,1,0,0,0,1],
            [1,0,1,1,1,1,0,1,0,0,0,1,0,1,1,1],
            [1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,1],
            [1,0,0,0,1,0,1,0,1,0,1,0,1,1,0,1],
            [1,0,1,0,1,0,1,1,1,0,1,0,1,0,0,1],
            [1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1],
            [1,0,1,0,1,0,0,0,1,1,1,0,0,0,0,1],
            [1,0,0,1,1,0,0,0,1,0,0,0,0,1,0,1],
            [1,0,0,0,0,0,1,0,0,1,0,0,1,1,0,1],
            [1,1,0,1,1,0,1,0,1,1,0,0,0,0,0,1],
            [1,0,0,0,1,0,0,0,0,0,0,1,1,0,1,1],
            [1,0,1,0,1,0,0,1,0,0,0,1,1,0,0,1],
            [1,1,1,0,0,0,1,1,1,0,1,1,0,0,0,1],
            [1,0,0,0,1,0,0,1,0,0,0,0,0,1,0,1],
            [1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1]
        ];
        List<string> texts = new List<string>() { 
            "./Content/testfloor.png", 
            "./Content/testceiling.png", 
            "./Content/testwalls.png" 
        };
        var player = new Entity();
        player.AddComponent(new Player((1, 2), 3));
        var entity = new Entity();
        entity.AddComponent(new DungeonRenderer(mapData,"./Models/atlas.json", texts, player.GetComponent<Player>()));
        AddEntity(entity);
        
    }

    public override void Update(GameTime gameTime)
    {
        
        base.Update(gameTime);
    }
}