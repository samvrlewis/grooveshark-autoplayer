using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
﻿using CSCore.CoreAudioAPI;

namespace grooveshark_autoplayer
{
    class AudioPlayingChecker
    {
        MMDevice device;

        public AudioPlayingChecker()
        {
            device = GetDefaultRenderDevice();
        }

        public static MMDevice GetDefaultRenderDevice()
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                return enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            }
        }

        public bool IsAudioPlaying()
        {
            using (var meter = AudioMeterInformation.FromDevice(this.device))
            {
                return meter.PeakValue > 0;
            }
        }
    }
}
