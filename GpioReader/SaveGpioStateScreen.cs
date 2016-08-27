using System;
using System.Threading;
using System.IO;
using WiringPiNet;
using System.Text;
using WiringPiNet.Tools;
using System.Collections.Generic;

namespace GpioTool
{
	public class SaveGpioStateScreen : FileDialogScreen
	{
		protected bool handsOffMode = false;
		protected Dictionary<int, string> tagsState = null;

		public override int Show(params object[] args)
		{
			if (args != null && args.Length > 0 && args[0] is Dictionary<int, string>)
			{
				tagsState = (Dictionary<int,string>)(args[0]);
			}

			return base.Show(args);
		}

		public void SaveAndExit(string path, bool handsoff)
		{
			handsOffMode = handsoff;
			OnPathEntered(path);
		}

		protected override string GetPrompt()
		{
			return "Enter file path to save:";
		}

		protected override int OnPathEntered(string path)
		{
			Console.Clear();
			Console.WriteLine("Dumping GPIO state to file : {0}.", path);

			SerializationTool tool = new SerializationTool();
			String serializedState = tool.SerializeState(0,MainClass.MAX_GPIO_NUMBER, tagsState);

			Console.WriteLine();
			Console.WriteLine(serializedState);
			Console.WriteLine();

			try 
			{
				File.WriteAllText(path, serializedState);
				Console.WriteLine("GPIO state dump completed, press any key to continue...");
				if (!handsOffMode)
				{
					Console.ReadKey(true);
				}
				return RETURN_VALUE_OK;
			}
			catch (Exception e)
			{
				Console.WriteLine(String.Format("Error while dumping GPIO state {0}, press any key to continue...", e.Message));
				if (!handsOffMode)
				{
					Console.ReadKey(true);
				}
				return RETURN_VALUE_ERROR;
			}


		}
	}
}

