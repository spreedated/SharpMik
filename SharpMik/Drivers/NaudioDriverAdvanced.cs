using NAudio.Wave;
using SharpMik.Player;
using System;

namespace SharpMik.Drivers
{
    class NAudioAdvancedTrackerStream : WaveStream
    {
        private readonly WaveFormat waveFormat;
        readonly NaudioDriverAdvanced m_Driver;
        public NAudioAdvancedTrackerStream(NaudioDriverAdvanced driver)
        {
            int bitness = (ModDriver.Mode & SharpMikCommon.DMODE_16BITS) == SharpMikCommon.DMODE_16BITS ? 16 : 8;
            int channels = (ModDriver.Mode & SharpMikCommon.DMODE_STEREO) == SharpMikCommon.DMODE_STEREO ? 2 : 1;

            waveFormat = new WaveFormat(ModDriver.MixFrequency, bitness, channels);

            m_Driver = driver;
        }

        public override long Position
        {
            get { return 0; }
            set {; }
        }

        public override long Length
        {
            get { return 0; }// { return _mixer.idxsize; }
        }

        public override WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return m_Driver.GetBuffer(buffer, offset, count);
        }
    }

    public class NaudioDriverAdvanced : VirtualSoftwareDriver
    {
        WaveOut waveOut;
        NAudioAdvancedTrackerStream m_NAudioStream;
        bool stopped = false;
        readonly Object mutext = new Object();

        public NaudioDriverAdvanced()
        {
            m_Next = null;
            m_Name = "NAudio Driver Advanced";
            m_Version = "NAudio 2.0";
            m_HardVoiceLimit = 0;
            m_SoftVoiceLimit = 255;
            m_AutoUpdating = true;
        }


        public override void CommandLine(string command)
        {

        }

        public int GetBuffer(byte[] buffer, int offset, int count)
        {
            lock (mutext)
            {
                uint done = 0;
                if (!stopped)
                {
                    sbyte[] buf = new sbyte[count];
                    done = WriteBytes(buf, (uint)count);
                    Buffer.BlockCopy(buf, 0, buffer, offset, count);
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        buffer[offset + i] = 0;
                    }
                    done = (uint)count;
                }

                return (int)done;
            }
        }

        public override bool IsPresent()
        {
            return true;
        }

        public override bool Init()
        {
            stopped = true;
            return base.Init();
        }

        public override void Exit()
        {

        }

        public override bool PlayStart()
        {
            lock (mutext)
            {
                if (waveOut == null)
                {
                    waveOut = new WaveOut();
                    waveOut.DesiredLatency = 250;
                    waveOut.DeviceNumber = 0;
                    m_NAudioStream = new NAudioAdvancedTrackerStream(this);
                    waveOut.Init(m_NAudioStream);
                }


                waveOut.Play();

                stopped = false;

                return base.PlayStart();
            }
        }

        public override void PlayStop()
        {
            lock (mutext)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
                base.PlayStop();
            }
        }

        public override void Update()
        {

        }

        public override void Pause()
        {
            waveOut.Pause();
        }

        public override void Resume()
        {
            waveOut.Play();
        }
    }
}
