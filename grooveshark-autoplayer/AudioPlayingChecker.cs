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

        // Checks if any audio is playing at all
        public bool IsAudioPlaying()
        {
            using (var meter = AudioMeterInformation.FromDevice(this.device))
            {
                return meter.PeakValue > 0;
            }
        }
        
        // Finds the number of applications making audio
        // In theory, should be able to use session.displayname to get the name of the app but this doesn't seem to work
        public static int NumberApplicationsPlaying()
        {
            List<float> volumes = new List<float>(); 

            using (var sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render))
            {
                using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                {
                    foreach (var session in sessionEnumerator)
                    {
                        using (var audioMeterInformation = session.QueryInterface<AudioMeterInformation>())
                        {
                            volumes.Add(audioMeterInformation.GetPeakValue());
                        }
                    }
                }
            }

            return volumes.Where(volume => volume > 0).Count();
        }

        private static AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                using (var device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia))
                {
                    var sessionManager = AudioSessionManager2.FromMMDevice(device);
                    return sessionManager;
                }
            }
        }



    }
}
