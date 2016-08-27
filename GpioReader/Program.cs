using System;

namespace GpioTool
{
	class MainClass
	{
		public const int MAX_GPIO_NUMBER = 40;

		public static void Main(string[] args)
		{
			if (args != null && args.Length > 0)
			{
				bool handsoff = false;
				foreach (string arg in args)
				{
					if (arg.StartsWith("--handsoff"))
					{
						handsoff = true;
					}
					else if (arg.StartsWith("--load"))
					{
						LoadGpioStateScreen loadScreen = new LoadGpioStateScreen();
						string path = GetArgumentValue("--load:", arg);
						if (path != null)
						{
							loadScreen.LoadAndExit(path, handsoff);
						}
						else
						{
							loadScreen.Show();
						}
						return;
					}
					else if (arg.StartsWith("--save"))
					{
						SaveGpioStateScreen saveScreen = new SaveGpioStateScreen();
						string path = GetArgumentValue("--save:", arg);
						if (path != null)
						{
							saveScreen.SaveAndExit(path, handsoff);
						}
						else
						{
							saveScreen.Show();
						}
						return;
					}
					else if (arg.StartsWith("--help"))
					{
						new HelpScreen().Show(handsoff);
						return;
					}
				}
			}

			IScreen currentScreen = new MainScreen();
			object[] showArgs = null;

			while (true)
			{
				int retValue = currentScreen.Show(showArgs);
				showArgs = null;

				if (currentScreen is MainScreen)
				{
					if (retValue == MainScreen.RETURN_VALUE_HELP)
					{
						currentScreen = new HelpScreen();
						continue;
					}
					else if (retValue == MainScreen.RETURN_VALUE_LOAD_FILE)
					{
						currentScreen = new LoadGpioStateScreen();
						continue;
					}
					else if (retValue == MainScreen.RETURN_VALUE_WRITE_FILE)
					{
						showArgs = new object[]{ MainScreen.tagsState };
						currentScreen = new SaveGpioStateScreen();
						continue;
					}
				}
				else if (currentScreen is HelpScreen)
				{
					currentScreen = new MainScreen();
					continue;
				}
				else if (currentScreen is LoadGpioStateScreen)
				{
					if (retValue == LoadGpioStateScreen.RETURN_VALUE_OK)
					{
						showArgs = new object[] { false, ((LoadGpioStateScreen)currentScreen).GetDeserializedTagsState() };
					}

					currentScreen = new MainScreen();
					continue;
				}
				else if (currentScreen is SaveGpioStateScreen)
				{
					currentScreen = new MainScreen();
					continue;
				}

				break;
			}

			Console.Clear();
		}

		protected static string GetArgumentValue(string prefix, string fullArgument)
		{
			int prefixLength = prefix.Length;
			int fullLength = fullArgument.Length;
			int cutOutLength = fullLength - prefixLength;

			if (cutOutLength > 0)
			{
				return fullArgument.Substring(prefixLength, cutOutLength);
			}
			else
			{
				return null;
			}
		}
	}
}
