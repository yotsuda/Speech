using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Speech.Recognition;
using System.Text;
using System.Threading;
using Voice.Cmdlets.Common;

namespace Voice.Cmdlets.Windows
{
    /// <summary>
    /// Continuous speech recognition using Windows Speech API.
    /// Shows real-time recognition with live updates. Press Enter to confirm and stop.
    /// </summary>
    [Cmdlet(VerbsCommunications.Read, "WindowsVoice")]
    [OutputType(typeof(string), typeof(PSObject[]))]
    public class ReadWindowsVoiceCmdlet : PSCmdlet
    {
        [Parameter]
        [ValidateRange(1, 300)]
        public int InitialTimeoutSeconds { get; set; } = 30;

        [Parameter]
        [ValidateRange(1, 60)]
        public int EndSilenceSeconds { get; set; } = 3;

        [Parameter]
        public string Language { get; set; } = System.Globalization.CultureInfo.CurrentCulture.Name;

        [Parameter]
        [ValidateRange(0.0, 1.0)]
        public double Confidence { get; set; } = 0.3;

        [Parameter]
        public SwitchParameter NoAutoStop { get; set; }

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        private readonly List<RecognitionResult> _results = new();
        private readonly object _lock = new();
        private bool _stopRequested = false;
        private DateTime _lastActivityTime = DateTime.MinValue;
        private bool _hasRecognizedSpeech = false;
        private string _currentHypothesis = "";
        private int _lastDisplayLength = 0;

        protected override void ProcessRecord()
        {
            using var recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo(Language));
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.LoadGrammar(new DictationGrammar());

            // Event handlers
            recognizer.SpeechHypothesized += OnSpeechHypothesized;
            recognizer.SpeechRecognized += OnSpeechRecognized;

            Console.WriteLine($"Starting speech recognition (Language: {Language})");
            if (NoAutoStop)
            {
                Console.WriteLine("Press Enter to confirm...");
            }
            else
            {
                Console.WriteLine($"Press Enter to confirm, or wait {EndSilenceSeconds} seconds of silence for auto-confirm...");
            }
            Console.WriteLine();
            Console.Write("> ");

            recognizer.RecognizeAsync(RecognizeMode.Multiple);

            var startTime = DateTime.Now;
            _lastActivityTime = startTime;

            // Monitor loop
            while (!_stopRequested)
            {
                // Check for Enter key
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        _stopRequested = true;
                        break;
                    }
                }

                // Check auto-stop condition
                if (!NoAutoStop && _hasRecognizedSpeech)
                {
                    lock (_lock)
                    {
                        var silenceDuration = (DateTime.Now - _lastActivityTime).TotalSeconds;
                        if (silenceDuration >= EndSilenceSeconds && string.IsNullOrEmpty(_currentHypothesis))
                        {
                            _stopRequested = true;
                            break;
                        }
                    }
                }

                // Check initial timeout
                if (!_hasRecognizedSpeech)
                {
                    var elapsed = (DateTime.Now - startTime).TotalSeconds;
                    if (elapsed >= InitialTimeoutSeconds)
                    {
                        _stopRequested = true;
                        break;
                    }
                }

                Thread.Sleep(50);
            }

            recognizer.RecognizeAsyncCancel();

            // Clear current line and move to new line
            ClearCurrentLine();
            Console.WriteLine();

            OutputResults();
        }

        private void OnSpeechHypothesized(object? sender, SpeechHypothesizedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Result.Text))
            {
                lock (_lock)
                {
                    _currentHypothesis = e.Result.Text;
                    _lastActivityTime = DateTime.Now;
                    _hasRecognizedSpeech = true;
                }

                DisplayHypothesis(e.Result.Text);
            }
        }

        private void OnSpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence < Confidence)
            {
                return;
            }

            var text = e.Result.Text;
            var duration = e.Result.Audio?.Duration.TotalSeconds ?? 0;

            // Filter out backchannels
            if (BackchannelDetector.IsBackchannel(text, duration))
            {
                return;
            }

            lock (_lock)
            {
                _results.Add(new RecognitionResult
                {
                    Text = text,
                    Confidence = e.Result.Confidence,
                    Duration = duration,
                    Timestamp = DateTime.Now
                });

                _currentHypothesis = "";
                _lastActivityTime = DateTime.Now;
                _hasRecognizedSpeech = true;
            }

            DisplayFinal(text);
        }

        private void DisplayHypothesis(string text)
        {
            var display = $"> {text}";
            var padding = _lastDisplayLength > display.Length 
                ? new string(' ', _lastDisplayLength - display.Length) 
                : "";
            
            Console.Write($"\r{display}{padding}");
            _lastDisplayLength = display.Length;
        }

        private void DisplayFinal(string text)
        {
            var display = $"> {text}";
            var padding = _lastDisplayLength > display.Length 
                ? new string(' ', _lastDisplayLength - display.Length) 
                : "";
            
            Console.WriteLine($"\r{display}{padding}");
            Console.Write("> ");
            _lastDisplayLength = 2;
        }

        private void ClearCurrentLine()
        {
            if (_lastDisplayLength > 0)
            {
                Console.Write($"\r{new string(' ', _lastDisplayLength)}\r");
            }
        }

        private void OutputResults()
        {
            if (_results.Count == 0)
            {
                Console.WriteLine("No recognition results.");
                WriteObject(null);
                return;
            }

            if (PassThru)
            {
                var output = new List<PSObject>();
                foreach (var result in _results)
                {
                    var obj = new PSObject();
                    obj.Properties.Add(new PSNoteProperty("Text", result.Text));
                    obj.Properties.Add(new PSNoteProperty("Confidence", result.Confidence));
                    obj.Properties.Add(new PSNoteProperty("Duration", result.Duration));
                    obj.Properties.Add(new PSNoteProperty("Timestamp", result.Timestamp));
                    output.Add(obj);
                }
                WriteObject(output.ToArray());
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var result in _results)
                {
                    if (sb.Length > 0) sb.Append(' ');
                    sb.Append(result.Text);
                }
                WriteObject(sb.ToString());
            }
        }

        private class RecognitionResult
        {
            public string Text { get; set; } = "";
            public double Confidence { get; set; }
            public double Duration { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}