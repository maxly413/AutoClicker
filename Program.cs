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
        public int MaxCats = 10;
        public int MaxHouses = 2;
        public bool holdingCat = false;
    }

    class GameState
    {
        // Method for creating and displaying all graphics
        static void RenderScreen(PlayerData player)
        {
            Graphics.BeginDrawing(); // Must begin the drawing as part of Raylib library

            // Draw text player variables to show current stats
            Graphics.DrawText($"Fish: {player.totalFish:F2}", (int)Window.GetCenter().X, 20, 40, Graphics.Gold);
            Graphics.DrawText($"Cats: {player.cats.Count}", 20,20, 40, Graphics.White);
            Graphics.DrawText($"Houses: {player.houses.Count}", 20, 50, 40, Graphics.Gold);

            // Draws UI elements such as buttons
            UI.Draw();

            // Loops through all currently created houses and draws them
            foreach (House house in player.houses)
            {
                house.DrawHouse();
            }

            // Loops through all currently created cats and draws them
            foreach (Cat cat in player.cats)
            {
                cat.DrawCat();
            }

            Graphics.EndDrawing(); // Must end the drawing as part of Raylib library
        }

        // This creates the clock, window and sets the fps to 60
        static void InitializeGame()
        {
            Clock.initClock(); 
            Window.InitWindow();
            Window.SetTargetFPS();
        }

        static void UpdateCats(float dt, PlayerData player)
        {
            foreach (var cat in player.cats) // Loop though all cats
            {
                if (cat.cooldown.IsReady(dt)) // Calls cooldown function which checks if cooldown is complete, if so it'll return true
                    player.totalFish += cat.gatherAmount; // Increments fish if the cooldown was complete
                cat.CatMovement(dt); // Updates the cat's position 
                cat.PickUpCat(player); // Checks if the player clicks on a cat, if so will check if the cat was released on a house which will assign the cat to that house
            }
        }

        // This method contains logic for when a house is bought, currently wip
        static void BuyHouse(PlayerData player)
        {
            if (player.houses.Count == 1) // Checks to see if this is the second house player has bought
            {
                player.MaxCats = 20; // Increases max cats player can hold
                player.houses.Add(new House()); // Create new house object
                player.houses[1].pos = new Vector2(430, 140); // Give new house object specific position
            }
            else if (player.houses.Count == 2) // Unused 3rd house
            {
                Console.WriteLine("hello");
            }

            UI.buttons[1].Enabled = player.houses.Count < player.MaxHouses; // Disables "buy house" button if player has reached maximum houses
        }

        // All player input logic is kept here
        static void HandleInput(PlayerData player)
        {
            if (Input.IsMouseButtonLeftClicked()) player.totalFish += 1.0; // Increments fish if mouse left clicked

            // Buy cat:
            if (player.totalFish >= 10 && // Checks if player has enough fish to cover cat cost
                player.cats.Count < player.MaxCats && // Check if player has enough room for cats
                UI.buttons[0].IsClicked()) //Checks to see if player clicks "buy cat" button
            {
                player.cats.Add(new Cat(1, 0.20f) { pos = new Vector2(400,300)}); // If cat is bought create new cat object for player
                player.totalFish -= 10; // decrement cost from player's fish
                UI.buttons[0].Enabled = player.cats.Count < player.MaxCats;
            }

            // Buy house:
            if (player.totalFish >= 10 && // Checks if player has enough fish to cover house cost
                player.houses.Count < player.MaxHouses && // Check if player has not reach house limit
                UI.buttons[1].IsClicked()) //Checks to see if player clicks "buy house" button
            {
                BuyHouse(player); // Calls buy house logic
                player.totalFish -= 10; // decrement cost from player's fish
            }
        }

        static void Main()
        {
            InitializeGame(); // Begins clock and opens game window

            float dt; // deltaTime variable, this variable keeps track of how much time has passed between frames (in sec) allowing us to have consistant time across the whole game (e.g. if we want to know if 1 second is passed we'd have to use, see cooldown logic for more details)

            PlayerData player = new PlayerData(); // Create player object
            player.houses.Add(new House { pos = new Vector2(100,200),  width = 200, height = 100 } ); // Create first house

            // Create buttons and add to UI static class
            UI.buttons.Add(new Button().CreateButton(100,20,20,400,Graphics.White,"Buy Cat")); 
            UI.buttons.Add(new Button().CreateButton(100,20,20,430,Graphics.White,"Buy House"));

            // while loop to check if window exit icon has not been clicked
            while (!Window.WindowShouldClose())
            {
                dt = Clock.GetDeltaTime(); // update deltaTime to store how much time has passed since last frame

                //Console.WriteLine(Raylib.GetMousePosition()); // Uncomment this to print mouse location X,Y to console

                // Runs following methdods ever frame
                HandleInput(player);
                UpdateCats(dt, player);
                RenderScreen(player);
            }
            Window.CloseWindow();
        }
    }
}