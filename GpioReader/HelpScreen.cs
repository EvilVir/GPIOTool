using System;
using WiringPiNet;

namespace GpioTool
{
	public class HelpScreen : IScreen
	{
		public HelpScreen ()
		{
		}

		public int Show(params object[] args)
		{
			return Show(false);
		}

		public int Show(bool handsOff)
		{
			Console.SetCursorPosition(0, 0);
			Console.Clear();

			Console.WriteLine("GpioTool (C) 2015 Kuba 'Vir' Pilecki");
			Console.WriteLine("-------------------------------------------------------------------------------------------------------------");
			Console.WriteLine();
			DisplayHelpLine("Escape", "Exit program");
			DisplayHelpLine("H", "This help screen");
			Console.WriteLine();
			DisplayHelpLine("[Minus]", "Step down probing interval by " + MainScreen.PIN_INTERVAL_CHANGE_STEP + "ms");
			DisplayHelpLine("[Plus]", "Step up probing interval by " + MainScreen.PIN_INTERVAL_CHANGE_STEP + "ms");
			Console.WriteLine();
			DisplayHelpLine("[Up]", "Select pin");
			DisplayHelpLine("[Down]", "Select pin");
			Console.WriteLine();
			DisplayHelpLine("0", "Set pin output to " + (PinValue.Low) + " mode");
			DisplayHelpLine("1", "Set pin output to " + (PinValue.High) + " mode");
			Console.WriteLine();
			DisplayHelpLine("I", "Set pin to " + (PinMode.Input) + " mode");
			DisplayHelpLine("O", "Set pin to " + (PinMode.Output) + " mode");
			DisplayHelpLine("P", "Set pin to " + (PinMode.PwmOutput) + " mode");
			DisplayHelpLine("C", "Set pin to " + (PinMode.GpioClock) + " mode");
			DisplayHelpLine("T", "Set pin to " + (PinMode.PwmToneOutput) + " mode");
			DisplayHelpLine("S", "Set pin to " + (PinMode.SoftPwmOutput) + " mode");
			DisplayHelpLine("X", "Set pin to " + (PinMode.SoftToneOutput) + " mode");
			Console.WriteLine();
			DisplayHelpLine("U", "Set pin resistor to " + (PullMode.Up) + " mode");
			DisplayHelpLine("D", "Set pin resistor to " + (PullMode.Down) + " mode");
			DisplayHelpLine("F", "Set pin resistor to " + (PullMode.Off) + " mode");
			Console.WriteLine();
			DisplayHelpLine("L", "Sets pins configuration from file dump");
			DisplayHelpLine("W", "Writes file dump with current pins configuration");
			Console.WriteLine();
			Console.WriteLine("-------------------------------------------------------------------------------------------------------------");
			Console.WriteLine("Command line options:");
			Console.WriteLine();
			DisplayHelpLine("--handsoff", "Handsoff mode for commands AFTER this argument");
			DisplayHelpLine("--help", "Displays this help screen");
			DisplayHelpLine("--load[:path]", "Displays load screen or (if :path supplied) loads file right away");
			DisplayHelpLine("--save[:path]", "Displays save screen or (if :path supplied) saves dump right away");
			Console.WriteLine();
			Console.WriteLine("-------------------------------------------------------------------------------------------------------------");
			Console.WriteLine("WARNING: THIS TOOL DOESN'T PREVENT BOARD NOR THINGS CONNECTED TO GPIO PORT FROM DAMAGE! USE AT YOUR OWN RISK");

			if (!handsOff)
			{
				while (true)
				{
					ConsoleKeyInfo keyinfo = Console.ReadKey(true);
					if (keyinfo.Key == ConsoleKey.Escape || keyinfo.Key == ConsoleKey.H)
					{
						return 0;
					}
				}
			}

			return 0;
		}

		protected void DisplayHelpLine(String key, String description)
		{
			Console.WriteLine(String.Format("{0,20} - {1}", key, description));
		}
	}
}

