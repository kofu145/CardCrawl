using System.Numerics;
using CardCrawl.Models;
using GramEngine.Core;
using GramEngine.Core.Input;
using GramEngine.ECS;
using GramEngine.ECS.Components;
using Newtonsoft.Json;

namespace CardCrawl.Components;

public class DungeonRenderer : Component
{
    private const int DEPTH = 4;
    private const int WIDTH = 3;
    private const int BASEWIDTH = 128;
    private const int BASEHEIGHT = 72;
    private const int MAPSIZE = 16;
    private (int x, int y)[] directions = [(0, -1), (1, 0), (0, 1), (-1, 0)];
    
    private DisplayFrame? mainFrame;
    private DisplayFrame? transitionFrame;

    private List<List<Tile>> atlas;
    private int[][] mapData;

    private TextureAtlas floorAtlas;
    private TextureAtlas ceilingAtlas;
    private TextureAtlas wallAtlas;

    private Player player;


    public DungeonRenderer(int[][] mapData, string atlasPath, List<string> texturePaths, Player player)
    {
        string jsonString = File.ReadAllText(atlasPath);
        atlas = JsonConvert.DeserializeObject<List<List<Tile>>>(jsonString);
        floorAtlas = new TextureAtlas(texturePaths[0]);
        ceilingAtlas = new TextureAtlas(texturePaths[1]);
        wallAtlas = new TextureAtlas(texturePaths[2]);
        this.mapData = mapData;
        this.player = player;
    }
    
    private (int px, int py) getDirOffset(int x, int z)
    {
        var px = 0;
        var py = 0;
        switch (player.Direction)
        {
            case 0: 
                px = player.DungeonX + x;
                py = player.DungeonY + z;
                break;
            case 1: 
                px = player.DungeonX  - z;
                py = player.DungeonY + x;
                break;
            case 2: 
                px = player.DungeonX  - x;
                py = player.DungeonY - z;
                break;
            case 3: 
                px = player.DungeonX  + z;
                py = player.DungeonY - x;
                break;
        }

        return (px, py);
    }

    private Entity getDraw(Tile tile, TextureAtlas targetAtlas)
    {
        var resEnt = new Entity();
        // scaling by 10x10 because base res is 10 below
        resEnt.Transform.Scale = new Vector2(10, 10);
        resEnt.AddComponent(targetAtlas.GetSpriteComponent(
            (BASEWIDTH, BASEHEIGHT), 
            (BASEWIDTH * tile.AtlasCol, BASEHEIGHT * tile.AtlasRow)
            ));

        if (tile.Flipped)
        {
            resEnt.Transform.Position.X = resEnt.Transform.Position.X + resEnt.GetComponent<Sprite>().Width * resEnt.Transform.Scale.X;
            resEnt.Transform.Scale.X = -resEnt.Transform.Scale.X;
        }

        return resEnt;
    }

    private void drawFloor(DisplayFrame frame, int z)
    {
        for (int x = -(WIDTH); x <= WIDTH; x++)
        {
            (int px, int py) = getDirOffset(x, -z);
            if ((px >= 0) && (py >= 0) && (px < MAPSIZE) && (py < MAPSIZE))
            {
                var result = CheckAtlas(0, x, z);

                if (result == null)
                    continue;
                var ent = getDraw(result, floorAtlas);
                frame.RenderSprite(ent);
                
            }
        }
        
    }
    private void drawCeiling(DisplayFrame frame, int z)
    {
        for (int x = -(WIDTH); x <= WIDTH; x++)
        {
            (int px, int py) = getDirOffset(x, -z);
            
            if ((px >= 0) && (py >= 0) && (px < MAPSIZE) && (py < MAPSIZE))
            {

                var result = CheckAtlas(1, x, z);
                if (result == null)
                    continue;
                var ent = getDraw(result, ceilingAtlas);
                frame.RenderSprite(ent);
                
            }
        }
    }
    private void drawSides(DisplayFrame frame, int z)
    {
        for (int x = -(WIDTH); x <= WIDTH; x++)
        {
            (int px, int py) = getDirOffset(x, -z);
            
            if ((px >= 0) && (py >= 0) && (px < MAPSIZE) && (py < MAPSIZE))
            {
                if (mapData[py][px] == 1)
                {
                    var result = CheckAtlas(2, x, z);
                    if (result == null)
                        continue;
                    var ent = getDraw(result, wallAtlas);
                    frame.RenderSprite(ent);
                }
            }
        }

    }
    private void drawFront(DisplayFrame frame, int z)
    {
        for (int x = -(WIDTH); x <= WIDTH; x++)
        {
            (int px, int py) = getDirOffset(x, -z);
            
            if ((px >= 0) && (py >= 0) && (px < MAPSIZE) && (py < MAPSIZE))
            {
                if (mapData[py][px] == 1)
                {
                    var result = CheckAtlas(3, 0, z);
                    if (result == null)
                        continue;
                    var ent = getDraw(result, wallAtlas);
                    ent.Transform.Position.X = ent.Transform.Position.X + x * result.FullWidth * 10;
                    frame.RenderSprite(ent);
                }
            }
        }
    }

    public Tile? CheckAtlas(int index, int x, int z)
    {
        var target = atlas[index].Find(e => e.X == x && e.Z == z);
        return target;
    }

    public void Render()
    {
        if (mainFrame != null)
        {
            mainFrame.StopRender();
            mainFrame.Dispose();
        }

        mainFrame = new DisplayFrame(
            GameStateManager.Window.settings.BaseWindowWidth, 
            GameStateManager.Window.settings.BaseWindowHeight
            );

        for (var z = DEPTH; z >= 0; z--)
        {
            // ceiling
            drawCeiling(mainFrame, z);

        }
        for (var z = DEPTH; z >= 0; z--)
        {
            // floor
            drawFloor(mainFrame, z);
        }
        for (var z = DEPTH; z >= 0; z--)
        {
            //sides/fronts
            drawSides(mainFrame, z);
            drawFront(mainFrame, z);

        }
        mainFrame.Render();
        
    }
    
    private bool canMoveTo(int x, int y)
    {
        return mapData[y][x] == 0;
    }
    
    private void moveForward()
    {
        var dirVector = directions[player.Direction];

        if (canMoveTo(player.DungeonX + dirVector.x, player.DungeonY + dirVector.y))
        {
            player.DungeonX += dirVector.x;
            player.DungeonY += dirVector.y;

            player.DungeonX = player.DungeonX < 1 ? player.DungeonX = 1 : player.DungeonX;
            player.DungeonY = player.DungeonY < 1 ? player.DungeonY = 1 : player.DungeonY;

            Render();
        }
    }

    private void moveBackward()
    {
        var dirVector = directions[player.Direction];

        if (canMoveTo(player.DungeonX - dirVector.x, player.DungeonY - dirVector.y))
        {
            player.DungeonX -= dirVector.x;
            player.DungeonY -= dirVector.y;

            player.DungeonX = player.DungeonX < 1 ? player.DungeonX = 1 : player.DungeonX;
            player.DungeonY = player.DungeonY < 1 ? player.DungeonY = 1 : player.DungeonY;

            Render();
        }
    }

    private void turnLeft()
    {
        
        player.Direction--;
        player.Direction = player.Direction < 0 ? 3 : player.Direction;
        Render();
    }

    private void turnRight()
    {
        player.Direction++;
        player.Direction = player.Direction > 3 ? 0 : player.Direction;
        Render();
    }
    
    public override void Initialize()
    {
        base.Initialize();
        Render();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (InputManager.GetKeyDown(Keys.W))
        {
            moveForward();
        }

        if (InputManager.GetKeyDown(Keys.A))
        {
            turnLeft();
        }

        if (InputManager.GetKeyDown(Keys.D))
        {
            turnRight();
        }

        if (InputManager.GetKeyDown(Keys.S))
        {
            moveBackward();
        }
    }
}