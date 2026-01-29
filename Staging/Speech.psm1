# Speech Module Initialization and Cleanup
# This script module provides cleanup functionality for the binary module

# Module removal handler - cleanup static resources
$MyInvocation.MyCommand.ScriptBlock.Module.OnRemove = {
    Write-Verbose "Cleaning up Speech resources..."

    try {
        # Cleanup Windows Audio Manager
        $windowsManager = [Speech.Cmdlets.Windows.WindowsAudioManager]
        if ($windowsManager) {
            $windowsManager::Cleanup()
            Write-Verbose "Windows Audio Manager cleaned up"
        }
    }
    catch {
        Write-Warning "Failed to cleanup Windows Audio Manager: $_"
    }

    try {
        # Cleanup Azure Audio Manager if it exists
        $azureManagerType = [Speech.Cmdlets.Azure.AzureAudioManager]
        if ($azureManagerType) {
            # Get the private static _instance field using reflection
            $field = $azureManagerType.GetField('_instance', [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Static)
            if ($field) {
                $mgr = $field.GetValue($null)
                if ($mgr) {
                    $mgr.Cleanup()
                    Write-Verbose "Azure Audio Manager cleaned up"
                }
            }
        }
    }
    catch {
        Write-Warning "Failed to cleanup Azure Audio Manager: $_"
    }

    Write-Verbose "Speech cleanup complete"
}
