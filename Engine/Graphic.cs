using System;
using System.Numerics;
using Raylib_cs;

namespace Engine
{
    public static class Graphics
    {
        public static readonly Color Gold = Color.Gold;
        public static readonly Color White = Color.White;
        public static readonly Color Red = Color.Red;

        public static void BeginDrawing()
        {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.LightGray); 
        }
        public static void EndDrawing() => Raylib.EndDrawing();
        public static void DrawText(string text, int posX, int posY, int fontSize, Color color) => Raylib.DrawText(text, posX, posY, fontSize, color);
    }

    public class Button
    {
        public int _width { get; set; }
        public int _height { get; set;}
        public int _posX { get; set; }
        public int _posY { get; set; }
        public Color _color { get; set; }
        public string _text { get; set; } = "";

        public bool Enabled { get; set; } = true;  

        public Button CreateButton(int width, int height, int posX, int posY, Color color, string text = "")
        {
            _width = width;
            _height = height;
            _posX = posX;
            _posY = posY;
            _color = color;
            _text = text;

            return this;
        }

        public void DrawButton()
        {
            if (!Enabled) return;

            Raylib.DrawRectangle(_posX, _posY, _width, _height, _color);
            Raylib.DrawText(_text, _posX, _posY, _height, Color.Black);
        }

        public bool IsClicked()
        {
            if (!Enabled) return false;

            if (Raylib.GetMouseX() >= _posX && Raylib.GetMouseX() <= _posX + _width
            && Raylib.GetMouseY() >= _posY && Raylib.GetMouseY() <= _posY + _height
            && Input.IsMouseButtonLeftClicked())
                return true;

            return false;
        }
    }

    public class TemporaryMessage
    {
        string _text = "";
        Vector2 _pos = new Vector2(0, 0);
        Cooldown _cooldown;
        Color _color = Color.Black;

        public TemporaryMessage(Vector2 pos, string text, float duration)
        {
            _pos = pos;
            _text = text;
            _cooldown = new Cooldown(duration);
        }

        public bool DrawMessage(float dt)
        {
            Raylib.DrawText(_text, (int)_pos.X, (int)_pos.Y, 20, _color);
            return _cooldown.IsReady(dt);
        }
    }

    static class UI
    {
        public static List<Button> buttons = new List<Button>();
        public static List<TemporaryMessage> temporaryMessages = new List<TemporaryMessage>();

        public static void Draw(float df)
        {
            foreach (var b in buttons)
            {
                b.DrawButton();
            }
            
            temporaryMessages.RemoveAll(m => m.DrawMessage(df));
        }
    }
}