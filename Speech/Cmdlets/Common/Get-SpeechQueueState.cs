using System.Management.Automation;
using Speech.Core;

namespace Speech.Cmdlets.Common
{
    /// <summary>
    /// 音声出力キューの現在の状態を取得
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SpeechQueueState")]
    [OutputType(typeof(VoiceQueueStateInfo))]
    public class GetVoiceQueueStateCmdlet : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            var state = new VoiceQueueStateInfo
            {
                QueueSize = SpeechState.QueueSize,
                IsPlaying = SpeechState.IsPlaying,
                IsQueueEmpty = SpeechState.IsQueueEmpty
            };

            WriteObject(state);
        }
    }

    /// <summary>
    /// キューの状態情報
    /// </summary>
    public class VoiceQueueStateInfo
    {
        public int QueueSize { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsQueueEmpty { get; set; }

        public override string ToString()
        {
            return $"QueueSize: {QueueSize}, IsPlaying: {IsPlaying}, IsEmpty: {IsQueueEmpty}";
        }
    }
}
