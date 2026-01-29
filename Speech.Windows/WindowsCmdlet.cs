using System;
using System.Management.Automation;

namespace Speech.Windows
{
    /// <summary>
    /// Base class for Windows-specific cmdlets that require Windows Speech API
    /// </summary>
    public abstract class WindowsCmdlet : PSCmdlet
    {
        /// <summary>
        /// Validates that the cmdlet is running on Windows platform
        /// </summary>
        /// <returns>True if running on Windows, false otherwise</returns>
        protected bool ValidateWindowsPlatform()
        {
            if (!WindowsAudioManager.IsWindowsPlatform())
            {
                WriteWarning("This cmdlet is only supported on Windows platform. Windows Speech API is not available on this operating system.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Override BeginProcessing to perform platform validation by default
        /// Derived classes can override this but should call base.BeginProcessing()
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (!ValidateWindowsPlatform())
            {
                // Stop pipeline execution if not on Windows
                throw new PlatformNotSupportedException("This cmdlet requires Windows platform and Windows Speech API.");
            }
        }
    }
}
