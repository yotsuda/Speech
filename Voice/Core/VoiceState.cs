using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Voice.Core
{
    /// <summary>
    /// 音声出力のグローバル状態管理
    /// </summary>
    public static class VoiceState
    {
        private static readonly ConcurrentQueue<WindowsVoiceRequest> _outputQueue = new();
        private static CancellationTokenSource? _currentCancellationTokenSource;
        private static int _isProcessing = 0;

        public static void EnqueueOutput(WindowsVoiceRequest request)
        {
            _outputQueue.Enqueue(request);

            if (Interlocked.CompareExchange(ref _isProcessing, 1, 0) == 0)
            {
                _ = Task.Run(ProcessQueueAsync);
            }
        }

        public static void ClearQueue()
        {
            _currentCancellationTokenSource?.Cancel();
            _currentCancellationTokenSource?.Dispose();
            _currentCancellationTokenSource = null;

            while (_outputQueue.TryDequeue(out _)) { }

            Interlocked.Exchange(ref _isProcessing, 0);
        }

        private static async Task ProcessQueueAsync()
        {
            try
            {
                while (_outputQueue.TryDequeue(out var request))
                {
                    _currentCancellationTokenSource = new CancellationTokenSource();

                    try
                    {
                        await request.PlayAsync(_currentCancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"[Voice TTS Error] {ex.Message}");
                    }
                    finally
                    {
                        _currentCancellationTokenSource?.Dispose();
                        _currentCancellationTokenSource = null;
                    }
                }
            }
            finally
            {
                Interlocked.Exchange(ref _isProcessing, 0);
            }
        }

        public static bool IsQueueEmpty => _outputQueue.IsEmpty;
        public static int QueueSize => _outputQueue.Count;
        public static bool IsPlaying => _isProcessing == 1;
    }
}
