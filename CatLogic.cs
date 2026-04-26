using Engine;
using Raylib_cs;
using System.Numerics;

namespace AutoClicker
{
    public class Cat
    {
        public float gatherAmount { get; set; } // Amount of fish gained at end of cooldown
        public Vector2 pos; // Vector2 position of cat on screen
        private bool _pickedUp = false; // Boolean variable to check if a cat is picked up (used to make sure cat is not dropped while left mouse is down)
        float _baseCooldown { get; set; } // The base cooldown which the cat will be reset to after leaving a house
        public Cooldown cooldown { get; set; } // Cooldown object allowing each cat to have it's own cooldown
        public House? house { get; set; }  // House object to assign a house to the cat
        public Color color { get; set; } = Color.Magenta; // Initial colour of cat (just for displaying purposes)
        int radius = 10; // Radius of cat (for displaying purposes + collision check)
        
        // Constructor method to create initial cat
        public Cat(float catCooldown, float gatherAmount) 
        {
            _baseCooldown = catCooldown;
            cooldown = new Cooldown(catCooldown);
            this.gatherAmount = gatherAmount;
        }

        // Method to reset cat once cat leaves house
        void ResetCat()
        {
            gatherAmount = 0.2f;
            cooldown = new Cooldown(_baseCooldown);
            color = Color.Magenta;
        }

        // Boolean function returns true is the cat is within the boundaries the passed in house 
        private bool IsInBoundry(House house)
        {
            return pos.X >= house.pos.X && 
                pos.X <= house.pos.X + house.width &&
                pos.Y >= house.pos.Y && 
                pos.Y <= house.pos.Y + house.height;
        }

        // Method for picking up and droping cats
        public void PickUpCat(PlayerData player)
        {
            // Checks if the player presses left click while mouse is on cat and is not already holding a cat
            if (Input.IsMouseButtonLeftClicked()
            && Vector2.Distance(Raylib.GetMousePosition(), pos) < radius
            && !player.holdingCat)
            {
                // Update holding variables
                _pickedUp =  true; 
                player.holdingCat = true; 

                if (house != null) house.Residents.Remove(this); // Removes the cat from a house if it was assigned to a house
                house = null; // Sets house to null to make sure the cat is properly not assigned to any houses (especially after player drops cat)
            }

            // Logic for when the player drops the cat
            if (Input.IsMouseButtonLeftUp() && _pickedUp) // Check to see if player was holding a cat and release left mouse
            {
                // Update holding variables
                _pickedUp =  false;
                player.holdingCat = false;

                house = null; // Initially set house to null (will be assigned in following code)
                foreach (House house in player.houses) // Loops though all houses
                {
                    if (IsInBoundry(house) && house.HasSpace) // For each house it checks if the cat's position is in the boundaries of a house
                    {
                        this.house = house; // If true set this house as the cat's house
                        break; // No longer needs to check further houses if house was found
                    }
                }

                // If a house was assigned then apply the house buffs using CatInside()
                if (house != null) house.CatInside(this);
                else ResetCat(); // If no house was found (meaning cat was dropped outside of the houses) reset the cat
            }

            if (_pickedUp) pos = Raylib.GetMousePosition(); // If the cat is currently being held make it follow player's cursor
        }

        // Logic for each cat's movement (currently wip, will later develop proper movement)
        public void CatMovement(float dt)
        {
            float speed = 50f;
            pos.X += Raylib.GetRandomValue(-1, 1) * speed * dt; // Multiply with dt as since this is being called evey frame without it the speed will scale with CPU processing speed
            pos.Y += Raylib.GetRandomValue(-1, 1) * speed * dt;

            if (house != null) // If the cat is assigned to a house make sure the cat can't leave the house
            {
                if (pos.X + radius > house.pos.X + house.width) pos.X = house.pos.X + house.width - radius;
                if (pos.Y + radius > house.pos.Y + house.height) pos.Y = house.pos.Y + house.height - radius;
                if (pos.X - radius < house.pos.X) pos.X = house.pos.X + radius;
                if (pos.Y - radius < house.pos.Y) pos.Y = house.pos.Y + radius;
            }
        }
        
        public void DrawCat()
        {
            Raylib.DrawEllipse((int)pos.X, (int)pos.Y, 10, 10, color);
        }
    }
}