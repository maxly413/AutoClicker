using System;
using Engine;

namespace AutoClicker
{
    public class Cat
    {
        public float amount { get; set; } = 0.2f;
        public float cost { get; set; } = 10;
        public float posX { get; set;} = 400;
        public float posY { get; set;} = 300;
        public Cooldown cooldown;
        
        public Cat(float catCooldown, float catCost)
        {
            cooldown = new Cooldown(catCooldown);
            cost = catCost;
        }
    }
}