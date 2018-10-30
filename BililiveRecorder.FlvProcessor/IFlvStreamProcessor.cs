﻿using System;
using System.Collections.ObjectModel;

namespace BililiveRecorder.FlvProcessor
{
    public interface IFlvStreamProcessor : IDisposable
    {
        event TagProcessedEvent TagProcessed;
        event StreamFinalizedEvent StreamFinalized;

        IFlvMetadata Metadata { get; set; }
        ObservableCollection<IFlvClipProcessor> Clips { get; }
        uint ClipLengthPast { get; set; }
        uint ClipLengthFuture { get; set; }

        IFlvStreamProcessor Initialize(Func<string> getStreamFileName, Func<string> getClipFileName, EnabledFeature enabledFeature);
        IFlvClipProcessor Clip();
        void AddBytes(byte[] data);
        void FinallizeFile();
    }
}