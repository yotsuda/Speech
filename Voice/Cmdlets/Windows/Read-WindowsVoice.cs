using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Speech.AudioFormat;
using System.Speech.Recognition;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using NAudio.Wave;
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

        [Parameter]
        [ArgumentCompleter(typeof(MicrophoneCompleter))]
        public string? Microphone { get; set; }

        private readonly List<RecognitionResult> _results = new();
        private readonly object _lock = new();
        private bool _stopRequested = false;
        private DateTime _lastActivityTime = DateTime.MinValue;
        private bool _hasRecognizedSpeech = false;
        private string _currentHypothesis = "";
        private int _lastDisplayLength = 0;
        private int _spinnerIndex = 0;
        private static readonly string[] _spinnerFrames = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
        private WaveInEvent? _waveIn;
        private AudioPipeStream? _audioStream;

        protected override void ProcessRecord()
        {
            using var recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo(Language));
            
            // Resolve microphone from parameter or config
            var microphoneName = ConfigManager.GetMicrophone(Microphone);
            
            if (!string.IsNullOrEmpty(microphoneName))
            {
                var micIndex = MicrophoneCompleter.FindMicrophoneIndex(microphoneName);
                if (micIndex == null)
                {
                    WriteWarning($"Microphone '{microphoneName}' not found. Using default microphone.");
                    recognizer.SetInputToDefaultAudioDevice();
                }
                else if (micIndex == 0)
                {
                    // Default microphone - use direct API for better performance
                    recognizer.SetInputToDefaultAudioDevice();
                    WriteVerbose($"Using default microphone: {microphoneName}");
                }
                else
                {
                    // Non-default microphone - use NAudio stream
                    SetupMicrophoneInput(recognizer, micIndex.Value);
                    WriteVerbose($"Using microphone: {microphoneName}");
                }
            }
            else
            {
                recognizer.SetInputToDefaultAudioDevice();
            }
            
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

            // Ensure UTF-8 output for spinner
            var originalEncoding = Console.OutputEncoding;
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            recognizer.RecognizeAsync(RecognizeMode.Multiple);

            var startTime = DateTime.Now;
            _lastActivityTime = startTime;

            // Monitor loop with spinner
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

                // Display spinner
                string hypothesis;
                lock (_lock)
                {
                    hypothesis = _currentHypothesis;
                }
                DisplaySpinner(hypothesis);

                Thread.Sleep(80);
            }

            recognizer.RecognizeAsyncCancel();
            StopMicrophoneCapture();

            // Clear current line and move to new line
            ClearCurrentLine();
            Console.WriteLine();
            Console.CursorVisible = true;
            Console.OutputEncoding = originalEncoding;

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

        private void DisplaySpinner(string text)
        {
            var spinner = _spinnerFrames[_spinnerIndex];
            _spinnerIndex = (_spinnerIndex + 1) % _spinnerFrames.Length;

            string display;
            if (string.IsNullOrEmpty(text))
            {
                display = $"> {spinner}";
            }
            else
            {
                display = $"> {text} {spinner}";
            }
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

        private void SetupMicrophoneInput(SpeechRecognitionEngine recognizer, int deviceIndex)
        {
            // Create pipe stream for producer-consumer pattern
            _audioStream = new AudioPipeStream();

            // Create NAudio WaveIn for specific microphone
            // Windows Speech API expects 16kHz, 16-bit, mono
            _waveIn = new WaveInEvent
            {
                DeviceNumber = deviceIndex,
                WaveFormat = new WaveFormat(16000, 16, 1)
            };

            _waveIn.DataAvailable += (sender, e) =>
            {
                if (e.BytesRecorded > 0)
                {
                    _audioStream.Write(e.Buffer, 0, e.BytesRecorded);
                }
            };

            _waveIn.StartRecording();

            // Set up audio format for Windows Speech API
            var audioFormat = new SpeechAudioFormatInfo(16000, AudioBitsPerSample.Sixteen, AudioChannel.Mono);
            recognizer.SetInputToAudioStream(_audioStream, audioFormat);
        }

        private void StopMicrophoneCapture()
        {
            _waveIn?.StopRecording();
            _waveIn?.Dispose();
            _waveIn = null;
            _audioStream?.Complete();
            _audioStream = null;
        }
    }

    /// <summary>
    /// A stream that supports concurrent reading and writing for audio piping.
    /// Producer (NAudio) writes audio data, Consumer (SpeechRecognitionEngine) reads it.
    /// </summary>
    internal class AudioPipeStream : Stream
    {
        private readonly BlockingCollection<byte[]> _chunks = new();
        private byte[] _currentChunk = Array.Empty<byte>();
        private int _currentChunkOffset = 0;
        private bool _completed = false;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => long.MaxValue;  // Infinite stream
        private long _position = 0;
        public override long Position
        {
            get => _position;
            set { }  // Ignore seeks
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_completed) return;
            
            var chunk = new byte[count];
            Array.Copy(buffer, offset, chunk, 0, count);
            _chunks.Add(chunk);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;

            while (bytesRead < count)
            {
                // Need more data from current chunk
                if (_currentChunkOffset >= _currentChunk.Length)
                {
                    // Try to get next chunk
                    if (_completed && _chunks.Count == 0)
                        break;

                    try
                    {
                        if (!_chunks.TryTake(out var chunk, 100))
                        {
                            if (_completed)
                                break;
                            continue;
                        }
                        _currentChunk = chunk;
                        _currentChunkOffset = 0;
                    }
                    catch (InvalidOperationException)
                    {
                        break;
                    }
                }

                // Copy from current chunk
                int available = _currentChunk.Length - _currentChunkOffset;
                int toCopy = Math.Min(available, count - bytesRead);
                Array.Copy(_currentChunk, _currentChunkOffset, buffer, offset + bytesRead, toCopy);
                _currentChunkOffset += toCopy;
                bytesRead += toCopy;
            }

            _position += bytesRead;
            return bytesRead;
        }

        public void Complete()
        {
            _completed = true;
            _chunks.CompleteAdding();
        }

        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => _position;  // Ignore seeks, return current position
        public override void SetLength(long value) => throw new NotSupportedException();
    }
}