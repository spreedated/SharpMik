using System;

using SharpMik.Interfaces;

namespace SharpMik.Player
{
	public partial class ModDriver
	{
		public static T LoadDriver<T>(Drivers.WavDriver.WavDriverOptions wavDriverOptions) where T : IModDriver, new()
		{
			m_Driver = new Drivers.WavDriver(wavDriverOptions);
			return (T)m_Driver;
		}
	}
}
