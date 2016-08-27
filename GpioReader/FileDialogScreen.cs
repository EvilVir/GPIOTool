using System;
using System.Text;

namespace GpioTool
{
	public abstract class FileDialogScreen : IScreen
	{
		public const int RETURN_VALUE_OK = 0;
		public const int RETURN_VALUE_ESCAPED = 1;
		public const int RETURN_VALUE_ERROR = -1;

		public FileDialogScreen ()
		{
		}

		public virtual int Show(params object[] args)
		{
			Console.Clear();
			Console.SetCursorPosition(0, 0);

			String prompt = GetPrompt();
			Console.WriteLine(GetPrompt());
			WritePromptLines(prompt.Length);
			Console.WriteLine();
			WritePromptLines(prompt.Length);
			string path = ConsoleHelper.ReadLineWithEscape(0, 2);

			if (path != null)
			{
				return OnPathEntered(path);
			}

			return RETURN_VALUE_ESCAPED;
		}

		protected void WritePromptLines(int length)
		{
			for (int i = 0; i < length; i++)
			{
				Console.Write('-');
			}

			Console.WriteLine();
		}

		protected abstract string GetPrompt();
		protected abstract int OnPathEntered(String path);
	}
}

