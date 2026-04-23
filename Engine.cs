using System;
using System.Diagnostics;
using Raylib_cs;

namespace Engine
{    public static class Input
    {
        public static bool IsMouseButtonLeftClicked()
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.Left)) return true;
            return false;
        }
    }

    public static class Clock
    {
        static double currentTime;
        static double lastTime = 0;
        static Stopwatch sw = new Stopwatch();

        public static void initClock()
        {
            sw.Start();
        }

        public static float GetDeltaTime()
        {
            currentTime = sw.Elapsed.TotalSeconds;
            float deltaTime = (float)(currentTime - lastTime);
            lastTime = currentTime;
            return deltaTime;
        }
    }

    public class Cooldown
    {
        public float Duration { get; set; }
        public float _timer = 0;
        public Cooldown(float seconds)
        {
            Duration = seconds;
        }   

        public bool IsReady(float dt)
        {
            _timer += dt;
            if (_timer >= Duration)
            {
                _timer = 0;
                return true;
            }
            return false;
        }
    }
}
