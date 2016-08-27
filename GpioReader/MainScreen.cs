using System;
using WiringPiNet.Tools;

using WiringPiNet.Wrapper;
using System.Collections.Generic;
using WiringPiNet;

namespace GpioTool
{
	public class MainScreen : IScreen
	{
		public const int PIN_START_INTERVAL = 100;
		public const int PIN_INTERVAL_CHANGE_STEP = 10;
		public const int PINS_LIST_OFFSET = 4;

		public const int RETURN_VALUE_EXIT = 0;
		public const int RETURN_VALUE_HELP = 1;
		public const int RETURN_VALUE_LOAD_FILE = 2;
		public const int RETURN_VALUE_WRITE_FILE = 3;

		protected int lastSelectedPin = -1;
		protected int selectedPin = -1;
		protected PinWatcher watcher;

		public static Dictionary<int, string> tagsState = new Dictionary<int, string>();

		public int Show(params object[] args)
		{
			using (Gpio gpio = new Gpio())
			{
				bool restoreTags = args != null && args.Length > 0 && args[0] is bool ? (bool)(args[0]) : true;
				if (args != null && args.Length > 1 && args[1] is Dictionary<int, string>)
				{
					tagsState = (Dictionary<int, string>)(args[1]);
					restoreTags = true;
				}

				InitializePins(gpio, restoreTags);
				SelectPin(0);
				InitializeConsole();

				while (true)
				{
					if (Console.KeyAvailable)
					{
						ConsoleKeyInfo keyInfo = Console.ReadKey(true);

						if (keyInfo.Key == ConsoleKey.Escape)
						{
							break;
						}
						else if (keyInfo.Key == ConsoleKey.UpArrow)
						{
							SelectPin(selectedPin - 1);
						}
						else if (keyInfo.Key == ConsoleKey.DownArrow)
						{
							SelectPin(selectedPin + 1);
						}
						else if (keyInfo.Key == ConsoleKey.I)
						{
							ChangePinMode(selectedPin, PinMode.Input);
						}
						else if (keyInfo.Key == ConsoleKey.O)
						{
							ChangePinMode(selectedPin, PinMode.Output);
						}
						else if (keyInfo.Key == ConsoleKey.P)
						{
							ChangePinMode(selectedPin, PinMode.PwmOutput);
						}
						else if (keyInfo.Key == ConsoleKey.C)
						{
							ChangePinMode(selectedPin, PinMode.GpioClock);
						}
						else if (keyInfo.Key == ConsoleKey.T)
						{
							ChangePinMode(selectedPin, PinMode.PwmToneOutput);
						}
						else if (keyInfo.Key == ConsoleKey.S)
						{
							ChangePinMode(selectedPin, PinMode.SoftPwmOutput);
						}
						else if (keyInfo.Key == ConsoleKey.X)
						{
							ChangePinMode(selectedPin, PinMode.SoftToneOutput);
						}
						else if (keyInfo.Key == ConsoleKey.U)
						{
							ChangePinPullMode(selectedPin, PullMode.Up);
						}
						else if (keyInfo.Key == ConsoleKey.D)
						{
							ChangePinPullMode(selectedPin, PullMode.Down);
						}
						else if (keyInfo.Key == ConsoleKey.F)
						{
							ChangePinPullMode(selectedPin, PullMode.Off);
						}
						else if (keyInfo.Key == ConsoleKey.D0)
						{
							WriteToPin(selectedPin, PinValue.Low);
						}
						else if (keyInfo.Key == ConsoleKey.D1)
						{
							WriteToPin(selectedPin, PinValue.High);
						}
						else if (keyInfo.KeyChar == '-')
						{
							ChangeFrequency(watcher.Interval - PIN_INTERVAL_CHANGE_STEP);
						}
						else if (keyInfo.KeyChar == '=')
						{
							ChangeFrequency(watcher.Interval + PIN_INTERVAL_CHANGE_STEP);
						}
						else if (keyInfo.Key == ConsoleKey.Enter)
						{
							EnterPinTag();
						}
						else if (keyInfo.Key == ConsoleKey.H)
						{
							FinalizePins();
							return RETURN_VALUE_HELP;
						}
						else if (keyInfo.Key == ConsoleKey.L)
						{
							FinalizePins();
							return RETURN_VALUE_LOAD_FILE;
						}
						else if (keyInfo.Key == ConsoleKey.W)
						{
							FinalizePins();
							return RETURN_VALUE_WRITE_FILE;
						}
					}
				}

				FinalizePins();
			}

			return RETURN_VALUE_EXIT;
		}

		protected void EnterPinTag()
		{
			string tag = ConsoleHelper.ReadLineWithEscape(44, selectedPin + PINS_LIST_OFFSET, 30);
			if (tag != null)
			{
				watcher.Get(selectedPin).Tag = tag;
			}
		}

		protected void InitializeConsole()
		{
			Console.SetCursorPosition(0, 0);
			Console.Clear();
			WriteStatus();

			foreach (GpioPin pin in watcher.GetAll())
			{
				WritePinState(pin);
			}
		}

		protected void ChangeFrequency(double newFrequency)
		{
			if (newFrequency > 0)
			{
				watcher.Interval = newFrequency;
				WriteStatus();
			}
		}

		protected void WriteToPin(int pinNumber, PinValue value)
		{
			GpioPin pin = watcher.Get(pinNumber);

			if (pin != null)
			{
				pin.Write(value);
				WritePinState(pin);
			}
		}

		protected void ChangePinMode(int pinNumber, PinMode mode)
		{
			GpioPin pin = watcher.Get(pinNumber);

			if (pin != null)
			{
				pin.SetMode(mode);
				WritePinState(pin);
			}
		}

		protected void ChangePinPullMode(int pinNumber, PullMode mode)
		{
			GpioPin pin = watcher.Get(pinNumber);

			if (pin != null)
			{
				pin.SetPullMode(mode);
				WritePinState(pin);
			}
		}

		protected void SelectPin(int pinNumber)
		{
			if (pinNumber >= 0 && pinNumber <= MainClass.MAX_GPIO_NUMBER)
			{
				lastSelectedPin = selectedPin;
				selectedPin = pinNumber;
				WritePinState(watcher.Get(pinNumber));
				if (lastSelectedPin > -1)
				{
					WritePinState(watcher.Get(lastSelectedPin));
				}
				WriteStatus();
			}
		}

		protected void InitializePins(Gpio gpio, bool restoreTags)
		{
			GpioPin[] pins = new GpioPin[MainClass.MAX_GPIO_NUMBER + 1];
			for (int i = 0; i <= MainClass.MAX_GPIO_NUMBER; i++)
			{
				pins[i] = gpio.GetPin(i);
				if (restoreTags && tagsState.ContainsKey(pins[i].Number))
				{
					pins[i].Tag = tagsState[pins[i].Number];
				}
				WritePinState(pins[i]);
			}

			tagsState.Clear();

			watcher = new PinWatcher(PIN_START_INTERVAL, pins);
			watcher.PinsStateChanged += PinsStateUpdate;
			watcher.Start();
		}

		protected void FinalizePins()
		{
			tagsState.Clear();
			foreach (GpioPin pin in watcher.GetAll())
			{
				if (pin.Tag != null)
				{
					tagsState.Add(pin.Number, pin.Tag);
				}
			}

			for (int i = 0; i <= MainClass.MAX_GPIO_NUMBER; i++)
			{
				watcher.RemoveAll();
			}

			watcher.Stop();
			watcher.PinsStateChanged -= PinsStateUpdate;
			watcher.Dispose();
		}

		protected void PinsStateUpdate(object sender, PinWatcher.PinsStateChangedEventArgs e)
		{
			if (e.Pins != null)
			{
				foreach (GpioPin pin in e.Pins)
				{
					WritePinState(pin);
				}
			}
		}

		protected void WriteStatus()
		{
			ConsoleHelper.WriteLine(0, "{0}ms | +/-: Change interval | Up/Down: Select pin | I/O/P/C/T/S/X: Pin mode | U/D/F: Pull mode | 0/1: Write value | Enter: Tag pin | H: Help | Esc: Exit",
				watcher.Interval);
			ConsoleHelper.WriteLine(1, "");
			ConsoleHelper.WriteLine(2, "{0,-9} | {1,-5} | {2,-14} | {3,-4} | {4} ", "Pin", "Value", "Mode", "Pull", "Tag");
			ConsoleHelper.WriteLine(3, "------------------------------------------------------------------------");
		}

		protected void WritePinState(GpioPin pin)
		{
			if (pin != null)
			{
				int pinPosition = pin.Number;
				ConsoleHelper.WriteLine(pinPosition + PINS_LIST_OFFSET, pinPosition == selectedPin ? ConsoleColor.Yellow : (ConsoleColor?)null, null, 
					"{0,3} / {1,-3} | {2,-5} | {3,-14} | {4,-4} | {5} ", 
					pin.Number,
					WiringPi.WpiPinToGpio(pin.Number),
					pin.CurrentValue, 
					pin.CurrentMode, 
					pin.PullMode,
					pin.Tag);
			}
		}


	}
}

