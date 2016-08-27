using System;
using System.Threading;
using System.IO;
using WiringPiNet.Tools;
using System.Collections.Generic;

namespace GpioTool
{
	public class LoadGpioStateScreen : FileDialogScreen
	{
		protected bool handsOffMode = false;
		protected Dictionary<int, string> tagsState;

		public void LoadAndExit(string path, bool handsoff)
		{
			handsOffMode = handsoff;
			OnPathEntered(path);
		}

		public Dictionary<int, string> GetDeserializedTagsState()
		{
			return tagsState;
		}

		protected override string GetPrompt()
		{
			return "Enter file path to load:";
		}

		protected override int OnPathEntered(string path)
		{
			Console.Clear();
			Console.WriteLine("Loading GPIO state from file : {0}.", path);
			Console.WriteLine();

			try
			{
				string serializedState = File.ReadAllText(path);
				Console.WriteLine(serializedState);
				Console.WriteLine();

				ConsoleKey? key = null;
				if (!handsOffMode)
				{
					Console.WriteLine("Press [Y] to accept and load this state or any other key to abort...");
					key = Console.ReadKey(true).Key;
				}

				if (handsOffMode || key == ConsoleKey.Y)
				{
					
					String log = (new SerializationTool()).DeserializeState(serializedState, out tagsState);
					Console.Clear();
					Console.WriteLine(log);
					Console.WriteLine();

					Console.WriteLine("State loaded, press any key to continue...");
					if (!handsOffMode)
					{
						Console.ReadKey(true);
					}
					return RETURN_VALUE_OK;
				}
				else
				{
					Console.WriteLine("Aborted, press any key to continue...");
					if (!handsOffMode)
					{
						Console.ReadKey(true);
					}
					return RETURN_VALUE_ESCAPED;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(String.Format("Error while deserializing GPIO state {0}, press any key to continue...", e.Message));
				if (!handsOffMode)
				{
					Console.ReadKey(true);
				}
				return RETURN_VALUE_ERROR;
			}
		}
	}
}

