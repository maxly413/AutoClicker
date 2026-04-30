using Engine;
using Raylib_cs;
using System.Numerics;
using System.Security;

namespace AutoClicker
{
    public class Cat : GameObject, IClickable
    {
        public float gatherAmount { get; set; } 
        public bool _pickedUp = false; 
        float _baseCooldown { get; set; } 
        public Cooldown cooldown { get; set; } 
        public House? house { get; set; } 
        private Texture2D _sprite;
        private Rectangle _bounds => new Rectangle(pos.X - _sprite.Width / 2, pos.Y - _sprite.Height / 2, _sprite.Width, _sprite.Height);
        public Color tint = Color.White;
        public bool IsPickedUp() => _pickedUp;
        
        
        public Cat(float catCooldown, float gatherAmount, Texture2D texture) 
        {
            _baseCooldown = catCooldown;
            cooldown = new Cooldown(catCooldown);
            this.gatherAmount = gatherAmount;
            _sprite = texture;
        }

        void ResetCat()
        {
            gatherAmount = 0.2f;
            cooldown = new Cooldown(_baseCooldown);
            if (house != null) house.Residents.Remove(this); 
            house = null;
            tint = Color.White;
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
        }

        public void PutDownCat(PlayerData player)
        {
            _pickedUp = false;
            player.holdingCat = false;
            
            // Store the old house temporarily
            House? oldHouse = this.house;
            this.house = null; 

            foreach (House h in player.houses)
            {
                if (IsInBoundry(h) && h.HasSpace)
                {
                    this.house = h; 
                    break; 
                }
            }
            
            if (this.house != null) 
            {
                // If we switched houses, remove from the old one!
                if (oldHouse != null && oldHouse != this.house) 
                {
                    oldHouse.Residents.Remove(this);
                }
                this.house.CatInside(this);
            }
            else 
            {
                // If we are in the wild, ResetCat handles the oldHouse removal
                this.house = oldHouse; // Temporarily put it back so ResetCat can find it
                ResetCat();
            }
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
                // Clamp X (Left and Right walls)
                // We use Math.Clamp to keep pos.X between (Left Wall + Radius) and (Right Wall - Radius)
                pos.X = Math.Clamp(pos.X, house.pos.X + _sprite.Width / 2, house.pos.X + house.width - _sprite.Width / 2);

                // Clamp Y (Top and Bottom walls)
                pos.Y = Math.Clamp(pos.Y, house.pos.Y + _sprite.Height / 2, house.pos.Y + house.height - _sprite.Height / 2);
            }
            else 
            {
                // Screen Clamping
                pos.X = Math.Clamp(pos.X, 0 + + _sprite.Width / 2, Raylib.GetScreenWidth() - _sprite.Width / 2);
                pos.Y = Math.Clamp(pos.Y, 0 + _sprite.Height / 2, Raylib.GetScreenHeight() - _sprite.Height / 2);
            }
        }
        
        public override void Draw()
        {
            // Use the bounds position for drawing so they always stay synced!
            Raylib.DrawTexture(_sprite, (int)_bounds.X, (int)_bounds.Y, tint);
            
            if (_pickedUp) 
                Graphics.DrawText($"Gather Amount: {gatherAmount}", (int)_bounds.X, (int)_bounds.Y - 15, 10, Color.White);
                
            //Raylib.DrawRectangleRec(_bounds, Raylib.Fade(Color.Magenta, 0.3f));
        }
            

        public bool IsMouseOver()
        {
            // Checks if mouse is within the radius of the cat's center
            return Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), _bounds);
        }

        public Action? OnClickAction;

        public void OnClick()
        {
            // "If I have an action assigned, run it!"
            OnClickAction?.Invoke(); //if (!player.holdingCat) newCat.PickUpCat(player); 
        }
    }
}