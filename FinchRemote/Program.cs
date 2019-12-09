using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.XInput;
using FinchAPI;
using HidSharp;
using System.Threading;

namespace FinchRemote
{
    class Program
    {
        static void Main(string[] args)
        {
            // create Finch
            Finch bot = new Finch();
            bot.connect();

            Console.WriteLine("Start XGamepadApp");
            // Initialize XInput
            var controllers = new[] { new Controller(UserIndex.One), new Controller(UserIndex.Two), new Controller(UserIndex.Three), new Controller(UserIndex.Four) };

            // Get 1st controller available
            Controller controller = null;
            foreach (var selectControler in controllers)
            {
                if (selectControler.IsConnected)
                {
                    controller = selectControler;
                    break;
                }
            }

            if (controller == null)
            {
                Console.WriteLine("No XInput controller installed");
            }
            else
            {
                Console.WriteLine("Found a XInput controller available");
                
                while (controller.IsConnected)
                {
                    if (IsKeyPressed(ConsoleKey.Escape))
                    {
                        break;
                    }
                    var state = controller.GetState();
                    var LeftJoyX = state.Gamepad.LeftThumbX;
                    var LeftJoyY = state.Gamepad.LeftThumbY;
                    //Console.WriteLine($"X: {LeftJoyX} Y: {LeftJoyY}");
                    int lmotor = (LeftJoyY / 128) + (LeftJoyX / 128);
                    int rmotor = (LeftJoyY / 128) - (LeftJoyX / 128);
                    bot.setMotors(lmotor, rmotor);
                    if (state.Gamepad.RightThumbY > 0 )
                    {
                        bot.noteOn(state.Gamepad.RightThumbY / 20);
                    }
                    if (state.Gamepad.RightThumbY < 0)
                    {
                        bot.noteOn((-(state.Gamepad.RightThumbY)) / 20);
                    }
                    int Red = 0;
                    int Green = 0;
                    int Blue = 0;
                    if (state.Gamepad.Buttons == GamepadButtonFlags.B)
                    {
                        Red = Red + 255;
                        Console.WriteLine($"Temperature: {bot.getTemperature()}");
                    }
                    if (state.Gamepad.Buttons == GamepadButtonFlags.A)
                    {
                        Green = Green + 255;
                        

                    }
                    if (state.Gamepad.Buttons == GamepadButtonFlags.X)
                    {
                        Blue = Blue + 255;
                    }
                    if (state.Gamepad.Buttons == GamepadButtonFlags.Y)
                    {
                        Red = Red + 255;
                        Green = Green + 255;
                        Console.WriteLine($"Light Level: {bot.getLeftLightSensor()}");
                    }
                    bot.setLED(Red, Green, Blue);
                    while (bot.isObstacleLeftSide() || bot.isObstacleRightSide())
                    {
                        bot.setMotors(-255, -255);
                    }

                }
            }
            Console.WriteLine("End XGamepadApp");
            bot.disConnect();
        }

        public static bool IsKeyPressed(ConsoleKey key)
        {
            return Console.KeyAvailable && Console.ReadKey(true).Key == key;
        }
    }
}
