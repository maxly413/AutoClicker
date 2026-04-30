using System;
using Engine;
using System.Collections.Generic;
using Raylib_cs;
using System.Numerics;

namespace AutoClicker
{
    public class PlayerData
    {
        public List<Cat> cats = new List<Cat>();
        public List<House> houses = new List<House>();
        public double totalFish = 0;
        public int MaxCats = 0;
        public int MaxHouses = 2;
        public bool holdingCat = false;
    }

    class GameState
    {
        static List<Texture2D> catTextures = new List<Texture2D>();
        static GameObject? GetObjectAtMouse(PlayerData player)
        {
            // Check cats first (because they are smaller/on top)
            foreach (var cat in player.cats)
            {
                if (cat.IsMouseOver())
                    return cat;
            }

            return null;
        }

        static void RenderScreen(PlayerData player, float dt)
        {
            Graphics.BeginDrawing();
            
            Graphics.DrawText($"Fish: {player.totalFish:F2}", (int)Window.GetCenter().X, 20, 40, Color.Black);
            Graphics.DrawText($"Cats: {player.cats.Count} / {player.MaxCats}", 20,20, 40, Color.Black);
            Graphics.DrawText($"Houses: {player.houses.Count}", 20, 50, 40, Color.Black);

            
            UI.Draw(dt);
           
            foreach (House house in player.houses)
            {
                house.Draw();
            }

            
            foreach (Cat cat in player.cats)
            {
                cat.Draw();
            }

            Graphics.EndDrawing(); 
        }

       
        static void InitializeGame()
        {
            Clock.initClock(); 
            Window.InitWindow();
            Window.SetTargetFPS();
        }

        static void UpdateCats(float dt, PlayerData player)
        {
            foreach (var cat in player.cats) 
            {
                if (cat.cooldown.IsReady(dt)) 
                    player.totalFish += cat.gatherAmount; 
                cat.CatMovement(dt);  
            }
        }
       
        static void BuyHouse(PlayerData player)
        {
            if (player.houses.Count == 0) 
            {
                player.houses.Add(new House { pos = new Vector2(100,200),  width = 200, height = 100, houseTint = Color.Yellow } ); 
                player.MaxCats += player.houses[0].Capacity; 
            }  
            else if (player.houses.Count == 1) 
            {
                player.houses.Add(new House() { pos = new Vector2(430, 140), width = 200, height = 100, houseTint = Color.Purple}); 
                player.MaxCats += player.houses[1].Capacity;
            }
            else if (player.houses.Count == 2) 
            {
                Console.WriteLine("hello");
            }

            UI.buttons[1].Enabled = player.houses.Count < player.MaxHouses; 
        }

        static void BuyCat(PlayerData player)
        {
            Cat newCat = new Cat(1, 0.20f, catTextures[0]) { pos = new Vector2(400, 300) };

                newCat.OnClickAction = () => { 
                    if (!player.holdingCat) newCat.PickUpCat(player); 
                };

                player.cats.Add(newCat);
        }

        static void HandleInput(PlayerData player)
        {
            if (Input.IsMouseButtonLeftClicked() && !player.holdingCat)
            {
                player.totalFish += 10;

                // Use your centralized logic to see what is under the mouse
                GameObject? clickedObject = GetObjectAtMouse(player);

                // "Is the thing I clicked a type of object that signs the IClickable contract?"
                if (clickedObject is IClickable interactable)
                {
                    interactable.OnClick();
                }
            }

            if (Raylib.IsMouseButtonReleased(MouseButton.Left))
            {
                if (player.holdingCat)
                {
                    // Find the cat that is currently picked up
                    Cat? heldCat = player.cats.Find(c => c.IsPickedUp());
                    heldCat?.PutDownCat(player);
                }
            }

            // Buy cat:
            if (player.totalFish >= 10 && 
                player.cats.Count < player.MaxCats && 
                UI.buttons[0].IsClicked()) 
            {
                BuyCat(player);
                player.totalFish -= 10; 
                UI.buttons[0].Enabled = player.cats.Count < player.MaxCats;
            }
            else if (player.totalFish >= 10 && player.cats.Count >= player.MaxCats && UI.buttons[0].IsClicked())
            {
                UI.temporaryMessages.Add(new TemporaryMessage(new Vector2(500,400), "Not enough houses", 2));
            }

            // Buy house:
            if (player.totalFish >= 10 && 
                player.houses.Count < player.MaxHouses && 
                UI.buttons[1].IsClicked()) 
            {
                BuyHouse(player); 
                player.totalFish -= 10; 
                UI.buttons[0].Enabled = player.cats.Count < player.MaxCats;
            }
        }

        static void Main()
        {
            InitializeGame();
            catTextures.Add(Raylib.LoadTexture("images/WhiteCat.png"));

            float dt; 
            PlayerData player = new PlayerData(); 

            UI.buttons.Add(new Button().CreateButton(100,20,20,400,Graphics.White,"Buy Cat")); 
            UI.buttons.Add(new Button().CreateButton(100,20,20,430,Graphics.White,"Buy House"));

            while (!Window.WindowShouldClose())
            {
                dt = Clock.GetDeltaTime(); 

                //Console.WriteLine(Raylib.GetMousePosition()); // Uncomment this to print mouse location X,Y to console
                
                HandleInput(player);
                UpdateCats(dt, player);
                RenderScreen(player, dt);
            }
            foreach (var t in catTextures) Raylib.UnloadTexture(t);            
            Window.CloseWindow();
        }
    }
}