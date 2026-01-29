using System;
using System.Collections.Generic;
using System.Management.Automation;
using NAudio.Wave;

namespace Speech.Cmdlets.Common
{
    /// <summary>
    /// Gets available microphone devices
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "Microphone")]
    [OutputType(typeof(MicrophoneInfo))]
    public class GetMicrophoneCmdlet : PSCmdlet
    {
        [Parameter]
        public SwitchParameter All { get; set; }

        protected override void ProcessRecord()
        {
            if (!OperatingSystem.IsWindows())
            {
                WriteWarning("Microphone enumeration is only supported on Windows.");
                return;
            }

            int deviceCount = WaveInEvent.DeviceCount;

            for (int i = 0; i < deviceCount; i++)
            {
                var capabilities = WaveInEvent.GetCapabilities(i);

                var info = new MicrophoneInfo
                {
                    Index = i,
                    Name = capabilities.ProductName,
                    Channels = capabilities.Channels
                };

                WriteObject(info);
            }

            if (deviceCount == 0)
            {
                WriteWarning("No microphone devices found.");
            }
        }
    }

    /// <summary>
    /// Microphone device information
    /// </summary>
    public class MicrophoneInfo
    {
        public int Index { get; set; }
        public string Name { get; set; } = "";
        public int Channels { get; set; }

        public override string ToString() => Name;
    }
}
