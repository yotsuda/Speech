using System.Management.Automation;
using Voice.Core;

namespace Voice.Cmdlets.Common
{
    /// <summary>
    /// 音声出力キューをクリア（Barge-in時に使用）
    /// </summary>
    [Cmdlet(VerbsCommon.Clear, "VoiceQueue", SupportsShouldProcess = true)]
    public class ClearVoiceQueueCmdlet : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            if (ShouldProcess("Voice Queue", "Clear all queued voice output"))
            {
                var queueSizeBefore = VoiceState.QueueSize;
                var wasPlaying = VoiceState.IsPlaying;

                VoiceState.ClearQueue();

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
