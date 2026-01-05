using System;
using System.Management.Automation;
using System.Speech.Recognition;
using System.Threading;
using Voice.Core;
using Voice.Cmdlets.Common;

namespace Voice.Cmdlets.Windows
{
    /// <summary>
    /// Windows音声認識でユーザー入力を待機
    /// Barge-in対応（-CancelOnSpeech）
    /// </summary>
    [Cmdlet(VerbsLifecycle.Wait, "WindowsVoiceInput")]
    public class WaitWindowsVoiceInputCmdlet : PSCmdlet
    {
        [Parameter]
        public int TimeoutSeconds { get; set; } = 60;

        [Parameter]
        public string Language { get; set; } = "ja-JP";

        [Parameter]
        [ValidateRange(0.0, 1.0)]
        public double Confidence { get; set; } = 0.7;

        [Parameter]
        public SwitchParameter CancelOnSpeech { get; set; }

        protected override void ProcessRecord()
        {
            using var recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo(Language));
            recognizer.SetInputToDefaultAudioDevice();

            // すべてを認識（制限なし）
            var dictationGrammar = new DictationGrammar();
            recognizer.LoadGrammar(dictationGrammar);

            string? recognizedText = null;
            var recognitionEvent = new ManualResetEvent(false);

            recognizer.SpeechRecognized += (s, e) =>
            {
                if (e.Result.Confidence < (float)Confidence)
                {
                    WriteVerbose($"Low confidence: {e.Result.Confidence} < {Confidence}");
                    return;
                }

                var text = e.Result.Text;
                var duration = e.Result.Audio.Duration.TotalSeconds;

                WriteVerbose($"Recognized: \"{text}\" (Confidence: {e.Result.Confidence}, Duration: {duration}s)");

                // Barge-in 判定
                if (CancelOnSpeech && VoiceState.IsPlaying)
                {
                    if (ShouldTriggerBargeIn(text, duration))
                    {
                        WriteVerbose($"Barge-in detected! Clearing queue...");
                        VoiceState.ClearQueue();
                    }
                    else
                    {
                        WriteVerbose($"Backchannel detected, ignoring: \"{text}\"");
                        return; // 相づちは無視
                    }
                }

                // 認識完了
                recognizedText = text;
                recognitionEvent.Set();
            };

            recognizer.RecognizeAsync(RecognizeMode.Multiple);

            // タイムアウト付き待機
            var timeout = TimeSpan.FromSeconds(TimeoutSeconds);
            if (recognitionEvent.WaitOne(timeout))
            {
                WriteObject(recognizedText);
            }
            else
            {
                WriteVerbose("Timeout");
                WriteObject(null);
            }

            recognizer.RecognizeAsyncCancel();
        }

        /// <summary>
        /// Barge-in をトリガーすべきか判定
        /// </summary>
        private bool ShouldTriggerBargeIn(string text, double duration)
        {
            // Use shared backchannel detection
            // If it's a backchannel, don't trigger barge-in
            if (BackchannelDetector.IsBackchannel(text, duration))
                return false;

            // Not a backchannel, trigger barge-in
            return true;
        }
    }
}
