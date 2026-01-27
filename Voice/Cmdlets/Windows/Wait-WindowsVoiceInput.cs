using System;
using System.Management.Automation;
using System.Speech.Recognition;
using Voice.Core;
using Voice.Cmdlets.Common;

namespace Voice.Cmdlets.Windows
{
    [Cmdlet(VerbsLifecycle.Wait, "WindowsVoiceInput")]
    public class WaitWindowsVoiceInputCmdlet : PSCmdlet
    {
        [Parameter]
        public int TimeoutSeconds { get; set; } = 30;

        [Parameter]
        public string Language { get; set; } = "ja-JP";

        [Parameter]
        [ValidateRange(0.0, 1.0)]
        public double Confidence { get; set; } = 0.3;

        [Parameter]
        public SwitchParameter CancelOnSpeech { get; set; }

        protected override void ProcessRecord()
        {
            using var recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo(Language));
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.LoadGrammar(new DictationGrammar());

            recognizer.InitialSilenceTimeout = TimeSpan.FromSeconds(TimeoutSeconds);
            recognizer.BabbleTimeout = TimeSpan.FromSeconds(10);

            Console.WriteLine($"音声認識中... ({TimeoutSeconds}秒でタイムアウト)");

            if (CancelOnSpeech && VoiceState.IsPlaying)
            {
                VoiceState.ClearQueue();
            }

            var result = recognizer.Recognize();

            if (result != null)
            {
                Console.WriteLine($"  > {result.Text} (信頼度: {result.Confidence:F2})");
                
                if (result.Confidence >= Confidence)
                {
                    var duration = result.Audio?.Duration.TotalSeconds ?? 0;
                    if (!BackchannelDetector.IsBackchannel(result.Text, duration))
                    {
                        WriteObject(result.Text);
                        return;
                    }
                }
            }
            
            Console.WriteLine("認識なし");
            WriteObject(null);
        }
    }
}
