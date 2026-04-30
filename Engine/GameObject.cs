using System.Numerics;
using Raylib_cs;

namespace Engine
{
    public interface IClickable
    {
        bool IsMouseOver();
        public void OnClick();
    }

    public abstract class GameObject
    {
        public Vector2 pos;
        public Color color = Color.White;

        public abstract void Draw();
    }
}