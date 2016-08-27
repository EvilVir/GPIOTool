using System;
using System.Text;

namespace GpioTool
{
	public class ConsoleHelper
	{
		protected static object consoleSync = new object();

		public static string ReadLineWithEscape(int left, int top, int? max = null)
		{
			StringBuilder sb = new StringBuilder();

			while (true)
			{
				ConsoleKeyInfo keyInfo = Console.ReadKey(true);
				if (keyInfo.Key == ConsoleKey.Escape)
				{
					return null;
				}
				else if (keyInfo.Key == ConsoleKey.Enter)
				{
					return sb.ToString();
				}
				else if (keyInfo.Key == ConsoleKey.Backspace)
				{
					if (sb.Length > 0)
					{
						sb.Remove(sb.Length - 1, 1);
					}
				}
				else
				{
					if (max == null || sb.Length < max)
					{
						sb.Append(keyInfo.KeyChar);
					}
				}

				PrintCurrentInput(sb.ToString(), left, top);
			}
		}

		protected static void PrintCurrentInput(String path, int left, int top)
		{
			Console.SetCursorPosition(left, top);
			Console.Write(String.Format("{0,-80}", path));
		}

		public static void WriteLine(int line, string text, params object[] args)
		{
			WriteLine(line, null, null, text, args);
		}

		public static void WriteLine(int line, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, string text, params object[] args)
		{
			lock (consoleSync)
			{
				String thisLineText = String.Format(text, args);
				Console.CursorVisible = false;
				if (foregroundColor != null)
				{
					Console.ForegroundColor = foregroundColor.Value;
				}

				if (backgroundColor != null)
				{
					Console.BackgroundColor = backgroundColor.Value;
				}

				Console.SetCursorPosition(0, line);
				Console.WriteLine(thisLineText);

				Console.ResetColor();
				Console.SetCursorPosition(0, 0);
				Console.CursorVisible = false;
			}
		}
	}
}

