using System;
using System.Diagnostics;
using Raylib_cs;

namespace Engine
{    public static class Input
    {
        public static bool IsMouseButtonLeftClicked()
        {
            return Raylib.IsMouseButtonPressed(MouseButton.Left);
        }

        public static bool IsMouseButtonLeftUp()
        {
            return Raylib.IsMouseButtonUp(MouseButton.Left);
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

        // Function for calculating dt which is the amount of time in seconds between frames
        public static float GetDeltaTime()
        {
            currentTime = sw.Elapsed.TotalSeconds; // Stores current time
            float deltaTime = (float)(currentTime - lastTime); // Calculates time since last frame by subtracing the current time with the time the last frame was at
            lastTime = currentTime; // Stores current time again for the next frame to use 
            return deltaTime;
        }
    }

    public class Cooldown
    {
        public float Duration { get; set; } // cooldown duration
        public float _timer = 0; // Cooldown's internal timer which accumulates how much time has passed
        public Cooldown(float seconds)
        {
            Duration = seconds;
        }   

        // Checks if cooldown is finished
        public bool IsReady(float dt)
        {
            _timer += dt; // Adds time passed since last fame to _timer
            if (_timer >= Duration) // If the time accumulating in total is >= cooldown duration the cooldown has ended
            {
                _timer = 0; // Reset the _timer to start new cooldown
                return true;
            }
            return false;
        }
    }
}
