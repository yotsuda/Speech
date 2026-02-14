using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Speech.Azure
{
    /// <summary>
    /// Registers a native library resolver so the Azure Speech SDK can find
    /// Microsoft.CognitiveServices.Speech.core.dll in the module's runtimes directory.
    /// PowerShell does not probe runtimes/{RID}/native/ automatically.
    /// </summary>
    public class ModuleInitializer : IModuleAssemblyInitializer
    {
        private static bool _resolverRegistered;

        public void OnImport()
        {
            if (_resolverRegistered)
                return;

            // The managed wrapper assembly should already be loaded as a dependency
            var sdkAssembly = Assembly.Load("Microsoft.CognitiveServices.Speech.csharp");
            NativeLibrary.SetDllImportResolver(sdkAssembly, ResolveSpeechNativeLibrary);
            _resolverRegistered = true;
        }

        private static IntPtr ResolveSpeechNativeLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (!libraryName.StartsWith("Microsoft.CognitiveServices.Speech", StringComparison.Ordinal))
                return IntPtr.Zero;

            var rid = RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => "win-x64",
                Architecture.Arm64 => "win-arm64",
                Architecture.X86 => "win-x86",
                _ => "win-x64"
            };

            var moduleDir = Path.GetDirectoryName(typeof(ModuleInitializer).Assembly.Location)!;
            var nativePath = Path.Combine(moduleDir, "runtimes", rid, "native", libraryName);

            if (!nativePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                nativePath += ".dll";

            if (NativeLibrary.TryLoad(nativePath, out var handle))
                return handle;

            return IntPtr.Zero;
        }
    }
}
