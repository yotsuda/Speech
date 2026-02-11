using System;
using System.Management.Automation;

namespace Speech.Windows
{
    [Cmdlet(VerbsCommon.Get, "WindowsSpeech")]
    public class GetWindowsVoiceCmdlet : WindowsCmdlet
    {
        [Parameter]
        [ArgumentCompleter(typeof(WindowsCultureCompleter))]
        public string? Culture { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var synthesizer = WindowsAudioManager.GetSynthesizer();
                var voices = synthesizer?.GetInstalledVoices();

                if (voices == null || voices.Count == 0)
                {
                    WriteWarning("No voices found. Make sure Windows Speech API is properly installed.");
                    return;
                }

                foreach (var voice in voices)
                {
                    var voiceInfo = voice.VoiceInfo;

                    // Culture filter
                    if (!string.IsNullOrEmpty(Culture) &&
                        !voiceInfo.Culture.Name.Equals(Culture, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    WriteObject(voiceInfo);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetVoicesError", ErrorCategory.InvalidOperation, null));
            }
        }
    }
}

