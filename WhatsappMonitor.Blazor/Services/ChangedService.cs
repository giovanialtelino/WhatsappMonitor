using System;

namespace WhatsappMonitor.Blazor.Services
{
    public class NewUploadService
    {
        public bool Upload { get; private set; }
        public event Action OnUpload;
        public void NewUpload()
        {
            Upload = true;
            NotifyStateChanged();
        }

        public void FinishUpload()
        {
            Upload = false;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnUpload?.Invoke();
    }
}

