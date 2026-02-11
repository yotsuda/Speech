using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Runtime.InteropServices;
using NAudio.Wave;

namespace Speech.Core
{
    /// <summary>
    /// Provides argument completion for audio output device names.
    /// Note: Output device selection via NAudio is only supported on Windows.
    /// </summary>
    public class OutputDeviceCompleter : IArgumentCompleter
    {
        public IEnumerable<CompletionResult> CompleteArgument(
            string commandName,
            string parameterName,
            string wordToComplete,
            CommandAst commandAst,
            IDictionary fakeBoundParameters)
        {
            var results = new List<CompletionResult>();

            if (!OperatingSystem.IsWindows())
                return results;

            int deviceCount = WaveInterop.waveOutGetNumDevs();

            for (int i = 0; i < deviceCount; i++)
            {
                string name = GetDeviceName(i);

                if (string.IsNullOrEmpty(wordToComplete) ||
                    name.Contains(wordToComplete, StringComparison.OrdinalIgnoreCase))
                {
                    // Quote the name if it contains spaces
                    string completionText = name.Contains(' ') ? $"'{name}'" : name;
                    results.Add(new CompletionResult(completionText, name, CompletionResultType.ParameterValue, name));
                }
            }

            return results;
        }

        /// <summary>
        /// Gets output device names for static access.
        /// Returns empty list on non-Windows platforms.
        /// </summary>
        public static List<string> GetOutputDeviceNames()
        {
            var names = new List<string>();

            if (!OperatingSystem.IsWindows())
                return names;

            int deviceCount = WaveInterop.waveOutGetNumDevs();

            for (int i = 0; i < deviceCount; i++)
            {
                names.Add(GetDeviceName(i));
            }

            return names;
        }

        /// <summary>
        /// Finds output device index by name (exact match first, then partial match).
        /// Returns -1 (default device) if name is null/empty or not found on non-Windows.
        /// Returns null if name is specified but no matching device is found.
        /// </summary>
        public static int? FindOutputDeviceIndex(string? name)
        {
            if (string.IsNullOrEmpty(name))
                return -1;

            if (!OperatingSystem.IsWindows())
                return -1;

            int deviceCount = WaveInterop.waveOutGetNumDevs();

            // Exact match first
            for (int i = 0; i < deviceCount; i++)
            {
                var deviceName = GetDeviceName(i);
                if (deviceName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            // Partial match
            for (int i = 0; i < deviceCount; i++)
            {
                var deviceName = GetDeviceName(i);
                if (deviceName.Contains(name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return null;
        }

        private static string GetDeviceName(int deviceIndex)
        {
            var caps = new WaveOutCapabilities();
            WaveInterop.waveOutGetDevCaps((IntPtr)deviceIndex, out caps,
                Marshal.SizeOf(typeof(WaveOutCapabilities)));
            return caps.ProductName;
        }
    }
}
