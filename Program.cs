using System;
using GramEngine.Core;

namespace CardCrawl
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WindowSettings windowSettings = new WindowSettings()
            {
                NaiveCollision = true,
                WindowTitle = "Pong demo",
                Width = 1280,
                Height = 720,
                SpriteCulling = true
            };
            
            Window window = new Window(new MainScene(), windowSettings);
            window.Run();
        }
    }
}