using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;
using NAudio.Wave;

namespace Speech.Cmdlets.Common
{
    /// <summary>
    /// Provides argument completion for microphone names.
    /// Note: Microphone selection via NAudio is only supported on Windows.
    /// </summary>
    public class MicrophoneCompleter : IArgumentCompleter
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

            int deviceCount = WaveInEvent.DeviceCount;

            for (int i = 0; i < deviceCount; i++)
            {
                var capabilities = WaveInEvent.GetCapabilities(i);
                string name = capabilities.ProductName;

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
        /// Gets microphone names for static access.
        /// Returns empty list on non-Windows platforms.
        /// </summary>
        public static List<string> GetMicrophoneNames()
        {
            var names = new List<string>();

            if (!OperatingSystem.IsWindows())
                return names;

            int deviceCount = WaveInEvent.DeviceCount;

            for (int i = 0; i < deviceCount; i++)
            {
                var capabilities = WaveInEvent.GetCapabilities(i);
                names.Add(capabilities.ProductName);
            }

            return names;
        }

        /// <summary>
        /// Finds microphone index by name (partial match).
        /// Returns null on non-Windows platforms.
        /// </summary>
        public static int? FindMicrophoneIndex(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            if (!OperatingSystem.IsWindows())
                return null;

            int deviceCount = WaveInEvent.DeviceCount;

            // Exact match first
            for (int i = 0; i < deviceCount; i++)
            {
                var capabilities = WaveInEvent.GetCapabilities(i);
                if (capabilities.ProductName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            // Partial match
            for (int i = 0; i < deviceCount; i++)
            {
                var capabilities = WaveInEvent.GetCapabilities(i);
                if (capabilities.ProductName.Contains(name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return null;
        }
    }
}
