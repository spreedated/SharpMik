using System;

using SharpMik.Interfaces;

namespace SharpMik.Player
{
	public partial class ModDriver
	{
		public static T LoadDriver<T>(Drivers.NaudioDriverAdvanced.NaudioDriverAdvacedOptions naudioDriverOptions) where T : IModDriver, new()
		{
			m_Driver = new Drivers.NaudioDriverAdvanced(naudioDriverOptions);
			return (T)m_Driver;
		}
	}
}
