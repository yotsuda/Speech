using System.Management.Automation;
using Speech.Core;

namespace Speech.Cmdlets.Common
{
    /// <summary>
    /// 音声出力キューをクリア（Barge-in時に使用）
    /// </summary>
    [Cmdlet(VerbsCommon.Clear, "SpeechQueue", SupportsShouldProcess = true)]
    public class ClearVoiceQueueCmdlet : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            if (ShouldProcess("Voice Queue", "Clear all queued voice output"))
            {
                var queueSizeBefore = SpeechState.QueueSize;
                var wasPlaying = SpeechState.IsPlaying;

                SpeechState.ClearQueue();

                WriteVerbose($"Cleared voice queue. Was playing: {wasPlaying}, Queue size before: {queueSizeBefore}");

                // 結果オブジェクトを返す
                var result = new PSObject();
                result.Properties.Add(new PSNoteProperty("WasPlaying", wasPlaying));
                result.Properties.Add(new PSNoteProperty("QueueSizeCleared", queueSizeBefore));

                WriteObject(result);
            }
        }
    }
}
