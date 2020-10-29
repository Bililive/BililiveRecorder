﻿using System;
using System.Collections.ObjectModel;

namespace BililiveRecorder.FlvProcessor
{
    public interface IFlvStreamProcessor : IDisposable
    {
        event TagProcessedEvent TagProcessed;
        event StreamFinalizedEvent StreamFinalized;
        event FlvMetadataEvent OnMetaData;

        int TotalMaxTimestamp { get; }
        int CurrentMaxTimestamp { get; }
        DateTime StartDateTime { get; }

        IFlvMetadata Metadata { get; set; }
        ObservableCollection<IFlvClipProcessor> Clips { get; }
        uint ClipLengthPast { get; set; }
        uint ClipLengthFuture { get; set; }
        uint CuttingNumber { get; set; }
        string path { set; get; }
        IFlvStreamProcessor Initialize(Func<string> getStreamFileName, Func<string> getClipFileName, EnabledFeature enabledFeature, AutoCuttingMode autoCuttingMode);
        IFlvClipProcessor Clip();
        void AddBytes(byte[] data);
        void FinallizeFile();
    }
}
