using System.Management.Automation;

namespace Speech.OpenAI
{
    public class OpenAISpeechInfo
    {
        public string Voice { get; set; } = "";
        public string Description { get; set; } = "";
    }

    [Cmdlet(VerbsCommon.Get, "OpenAISpeech")]
    [OutputType(typeof(OpenAISpeechInfo))]
    public class GetOpenAISpeechCmdlet : PSCmdlet
    {
        private static readonly Dictionary<string, string> VoiceDescriptions = new()
        {
            ["alloy"] = "Neutral and balanced",
            ["ash"] = "Soft and refined",
            ["ballad"] = "Warm and expressive",
            ["coral"] = "Clear and articulate",
            ["echo"] = "Smooth and resonant",
            ["fable"] = "Warm and narrative",
            ["onyx"] = "Deep and authoritative",
            ["nova"] = "Friendly and upbeat",
            ["sage"] = "Calm and measured",
            ["shimmer"] = "Bright and energetic",
            ["verse"] = "Versatile and dynamic"
        };

        protected override void ProcessRecord()
        {
            foreach (var voice in OpenAIAudioManager.AvailableVoices)
            {
                var info = new OpenAISpeechInfo
                {
                    Voice = voice,
                    Description = VoiceDescriptions.GetValueOrDefault(voice, "")
                };
                WriteObject(info);
            }
        }
    }
}
