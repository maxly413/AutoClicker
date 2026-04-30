using Engine;
using Raylib_cs;
using System.Numerics;

namespace AutoClicker
{
    public class Cat : GameObject, IClickable
    {
        public float gatherAmount { get; set; } 
        public bool _pickedUp = false; 
        float _baseCooldown { get; set; } 
        public Cooldown cooldown { get; set; } 
        public House? house { get; set; } 
        public int radius = 10; 

        public bool IsPickedUp() => _pickedUp;
        
        
        public Cat(float catCooldown, float gatherAmount) 
        {
            _baseCooldown = catCooldown;
            cooldown = new Cooldown(catCooldown);
            this.gatherAmount = gatherAmount;
            this.color = Color.Magenta;
        }

        void ResetCat()
        {
            gatherAmount = 0.2f;
            cooldown = new Cooldown(_baseCooldown);
            color = Color.Magenta;
        }

        private bool IsInBoundry(House house)
        {
            return pos.X >= house.pos.X && 
                pos.X <= house.pos.X + house.width &&
                pos.Y >= house.pos.Y && 
                pos.Y <= house.pos.Y + house.height;
        }

        public void PickUpCat(PlayerData player)
        {
            _pickedUp =  true; 
            player.holdingCat = true; 
            
            if (house != null) house.Residents.Remove(this); 
            house = null;
        }

        public void PutDownCat(PlayerData player)
        {
            _pickedUp =  false;
            player.holdingCat = false;
            house = null; 
            

            foreach (House house in player.houses)
            {
                if (IsInBoundry(house) && house.HasSpace)
                {
                    this.house = house; 
                    break; 
                }
            }
            
            if (house != null) house.CatInside(this);
            else ResetCat();
        }
      
        public void CatMovement(float dt)
        {
            if (_pickedUp)
            {
                pos = Raylib.GetMousePosition(); 
                return;
            }

            float speed = 50f;
            pos.X += Raylib.GetRandomValue(-1, 1) * speed * dt; 
            pos.Y += Raylib.GetRandomValue(-1, 1) * speed * dt;

            if (house != null) 
            {
                if (pos.X + radius > house.pos.X + house.width) pos.X = house.pos.X + house.width - radius;
                if (pos.Y + radius > house.pos.Y + house.height) pos.Y = house.pos.Y + house.height - radius;
                if (pos.X - radius < house.pos.X) pos.X = house.pos.X + radius;
                if (pos.Y - radius < house.pos.Y) pos.Y = house.pos.Y + radius;
            }
        }
        
        public override void Draw()
        {
            Raylib.DrawEllipse((int)pos.X, (int)pos.Y, radius, radius, color);
            if (_pickedUp) Graphics.DrawText($"Gather Amount: {gatherAmount}", (int)pos.X - 20, (int)pos.Y - 25, 10, Color.White);
        }

        public bool IsMouseOver()
        {
            // Checks if mouse is within the radius of the cat's center
            return Raylib.CheckCollisionPointCircle(Raylib.GetMousePosition(), pos, radius);
        }

        public Action? OnClickAction;

        public void OnClick()
        {
            // "If I have an action assigned, run it!"
            OnClickAction?.Invoke(); //if (!player.holdingCat) newCat.PickUpCat(player); 
        }
    }
}