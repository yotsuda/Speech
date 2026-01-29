using NAudio.Wave;

namespace Speech.Core
{
    /// <summary>
    /// Helper class for microphone device selection
    /// </summary>
    public static class MicrophoneHelper
    {
        /// <summary>
        /// Gets the device number for a microphone by name (partial match)
        /// </summary>
        /// <param name="microphoneName">Microphone name to search for (partial match)</param>
        /// <returns>Device number, or 0 if not found</returns>
        public static int GetDeviceNumber(string? microphoneName)
        {
            if (string.IsNullOrEmpty(microphoneName))
                return 0;

            for (int i = 0; i < WaveInEvent.DeviceCount; i++)
            {
                var caps = WaveInEvent.GetCapabilities(i);
                if (caps.ProductName.Contains(microphoneName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return 0; // Default device
        }
    }
}
