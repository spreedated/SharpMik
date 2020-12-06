using SharpMik.Extentions;
using SharpMik.Player;
using System;
using System.IO;

namespace SharpMik.Drivers
{
    public class WavDriver : VirtualSoftwareDriver
    {
        readonly WavDriverOptions m_DriverOptions = new WavDriverOptions();

        BinaryWriter m_FileStream;

        sbyte[] m_Audiobuffer;

        public static uint BUFFERSIZE = 32768;
        uint dumpsize;

        private void ConstructorInit()
        {
            m_Next = null;
            m_Name = "Disk Wav Writer";
            m_Version = "Wav disk writer (music.wav) v1.1";
            m_HardVoiceLimit = 0;
            m_SoftVoiceLimit = 255;
            m_AutoUpdating = false;
        }

        public WavDriver()
        {
            ConstructorInit();
        }

        public WavDriver(WavDriverOptions wavDriverOptions)
        {
            ConstructorInit();
            this.m_DriverOptions = wavDriverOptions;
        }

        public override void CommandLine(string command)
        {
            if (!String.IsNullOrEmpty(command))
            {
                m_DriverOptions.OutputFilename = command;
            }
        }

        public override bool IsPresent()
        {
            return true;
        }

        public override bool Init()
        {
            if (!m_DriverOptions.Overwrite && File.Exists(m_DriverOptions.OutputFilename))
            {
                throw new Exception("File exists, overwrite false");
            }
            try
            {
                FileStream stream = new FileStream(m_DriverOptions.OutputFilename, FileMode.Create);
                m_FileStream = new BinaryWriter(stream);
                m_Audiobuffer = new sbyte[BUFFERSIZE];

                ModDriver.Mode = (ushort)(ModDriver.Mode | SharpMikCommon.DMODE_SOFT_MUSIC | SharpMikCommon.DMODE_SOFT_SNDFX);

                Putheader();
                return base.Init();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public override void Exit()
        {
            try
            {
                Putheader();
                base.Exit();
                //putheader();
                m_FileStream.Close();
                m_FileStream.Dispose();
                m_FileStream = null;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }
        int loc = 0;

        public override void Update()
        {
            uint done = WriteBytes(m_Audiobuffer, BUFFERSIZE);
            m_FileStream.Write(m_Audiobuffer, 0, (int)done);
            dumpsize += done;
            loc++;
        }


        void Putheader()
        {
            m_FileStream.Seek(0, SeekOrigin.Begin);
            m_FileStream.Write("RIFF".ToCharArray());
            m_FileStream.Write((uint)(dumpsize + 44));
            m_FileStream.Write("WAVEfmt ".ToCharArray());
            m_FileStream.Write((uint)16);
            m_FileStream.Write((ushort)1);
            ushort channelCount = (ushort)((ModDriver.Mode & SharpMikCommon.DMODE_STEREO) == SharpMikCommon.DMODE_STEREO ? 2 : 1);
            ushort numberOfBytes = (ushort)((ModDriver.Mode & SharpMikCommon.DMODE_16BITS) == SharpMikCommon.DMODE_16BITS ? 2 : 1);

            m_FileStream.Write(channelCount);
            m_FileStream.Write((uint)ModDriver.MixFrequency);
            int blah = ModDriver.MixFrequency * channelCount * numberOfBytes;
            m_FileStream.Write((uint)(blah));
            m_FileStream.Write((ushort)(channelCount * numberOfBytes));
            m_FileStream.Write((ushort)((ModDriver.Mode & SharpMikCommon.DMODE_16BITS) == SharpMikCommon.DMODE_16BITS ? 16 : 8));
            m_FileStream.Write("data".ToCharArray());
            m_FileStream.Write((uint)dumpsize);
        }

        public class WavDriverOptions
        {
            public string OutputFilename { get; set; } = "music.wav";
            public bool Overwrite { get; set; }
        }
    }
}
