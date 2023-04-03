using System;
using SharpMik.Interfaces;

namespace SharpMik.Player
{
	public partial class MikMod
	{
		public bool Init<T>(Drivers.WavDriver.WavDriverOptions wavDriverOptions) where T : IModDriver, new()
		{
			loaded_Driver = ModDriver.LoadDriver<T>(wavDriverOptions);
			return ModDriver.MikMod_Init("");
		}
	}
}
