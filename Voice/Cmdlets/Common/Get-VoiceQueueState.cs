using System.Management.Automation;
using Voice.Core;

namespace Voice.Cmdlets.Common
{
    /// <summary>
    /// 音声出力キューの現在の状態を取得
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "VoiceQueueState")]
    [OutputType(typeof(VoiceQueueStateInfo))]
    public class GetVoiceQueueStateCmdlet : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            var state = new VoiceQueueStateInfo
            {
                QueueSize = VoiceState.QueueSize,
                IsPlaying = VoiceState.IsPlaying,
                IsQueueEmpty = VoiceState.IsQueueEmpty
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
