using System;
using SharpMik.Interfaces;

namespace SharpMik.Player
{
	public partial class MikMod
	{
		public bool Init<T>(Drivers.NaudioDriverAdvanced.NaudioDriverAdvacedOptions naudioDriverOptions) where T : IModDriver, new()
		{
			loaded_Driver = ModDriver.LoadDriver<T>(naudioDriverOptions);
			return ModDriver.MikMod_Init("");
		}
	}
}
