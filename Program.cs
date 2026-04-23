using System;
using Engine;
using System.Collections.Generic;
using Raylib_cs;

namespace AutoClicker
{
    public class PlayerData
    {
        public List<Cat> cats = new List<Cat>();
        public double totalFish = 0;
    }

    class AutoClicker
    {
        static void RenderScreen(PlayerData player)
        {
            Graphics.BeginDrawing();
            Graphics.DrawText($"Fish: ${player.totalFish:F2}", (int)Window.GetCenter().X, 20, 40, Graphics.Gold);
            Graphics.DrawText($"Cats: {player.cats.Count}", 20,20, 40, Graphics.Gold);
            UI.Draw();

            Raylib.DrawRectangleLines(200,200,400,200,Graphics.White);
            foreach (Cat cat in player.cats)
            {
                Raylib.DrawEllipse((int)cat.posX, (int)cat.posY, 10, 10, Color.Magenta);
                cat.posX += Raylib.GetRandomValue(-1, 1);
                cat.posY += Raylib.GetRandomValue(-1, 1);
            }

            Graphics.EndDrawing();
        }

        static void InitializeGame()
        {
            Clock.initClock();
            Window.InitWindow();
            Window.SetTargetFPS();
        }

        static void Main()
        {
            InitializeGame();

            float dt;

            PlayerData player = new PlayerData();

            UI.buttons.Add(new Button().CreateButton(100,20,20,70,Graphics.White,"Buy Cat"));

            while (!Window.WindowShouldClose())
            {
                dt = Clock.GetDeltaTime();

                //Console.WriteLine(Raylib.GetMousePosition());
                
                if (Input.IsMouseButtonLeftClicked()) player.totalFish += 1.0;

                if (player.totalFish >= 10 && UI.buttons[0].IsClicked())
                {
                    player.cats.Add(new Cat(1, 10));
                    player.totalFish -= 10;
                }
                
                foreach (var cat in player.cats)
                {
                    if (cat.cooldown.IsReady(dt)) player.totalFish += cat.amount;
                }
                RenderScreen(player);
            }
            Window.CloseWindow();
        }
    }
}