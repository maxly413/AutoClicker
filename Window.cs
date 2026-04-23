using Raylib_cs;
using System.Numerics;

namespace Engine
{
    public static class Window
    {
        public static void InitWindow() => Raylib.InitWindow(800, 450, "Auto Clicker");
        public static void SetTargetFPS() => Raylib.SetTargetFPS(60);
        public static bool WindowShouldClose() => Raylib.WindowShouldClose();
        public static void CloseWindow() => Raylib.CloseWindow();
        public static Vector2 GetCenter() => Raylib.GetScreenCenter();
    }
}