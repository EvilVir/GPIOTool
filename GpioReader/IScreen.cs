using System;

namespace GpioTool
{
	public interface IScreen
	{
		int Show(params object[] args);
	}
}

