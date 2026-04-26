using Engine;
using System.Numerics;
using Raylib_cs;

namespace AutoClicker
{
    public class House
    {
        public Vector2 pos;
        public int width { get; set; } = 200;
        public int height { get; set; } = 100;
        public int Capacity { get; set; } = 10; // Used to check if house hasSpace
              public List<Cat> Residents { get; set; } = new List<Cat>(); // List of cats inside of house (used to check how many cats are currently inside the house for checking if the house hasSpace)
        public bool HasSpace => Residents.Count < Capacity; // => operator (expression-bodied member) means that HasSpace updates every single time you look at it

        public void DrawHouse()
        {
            Raylib.DrawRectangleLines((int)pos.X, (int)pos.Y, width, height, Graphics.White);
        }

        // Method for updating cat & house when a cat is assigned to house
        public void CatInside(Cat cat)
        {
            cat.gatherAmount = 1; // Increase gatherAmount of cat
            cat.color = Color.Yellow; // Changes cat's colour

            if (!Residents.Contains(cat)) Residents.Add(cat); // Adds cat to residents in not already added
        }
    }
}