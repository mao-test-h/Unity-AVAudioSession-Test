using System;

namespace AVAudioSessionBridge
{
    public enum AudioSessionCategory
    {
        Ambient,
        SoloAmbient,
        Playback,
        Record,
        PlayAndRecord,
        MultiRoute
    }

    public enum AudioSessionMode
    {
        Default,
        VoiceChat,
        GameChat,
        VideoRecording,
        Measurement,
        MoviePlayback,
        VideoChat,
        SpokenAudio,
        VoicePrompt
    }

    [Flags]
    public enum AudioSessionCategoryOptions
    {
        None = 0,
        MixWithOthers = 1 << 0,
        DuckOthers = 1 << 1,
        AllowBluetooth = 1 << 2,
        DefaultToSpeaker = 1 << 3,
        InterruptSpokenAudioAndMixWithOthers = 1 << 4,
        AllowBluetoothA2DP = 1 << 5,
        AllowAirPlay = 1 << 6,
        OverrideMutedMicrophoneInterruption = 1 << 7
    }

    [Flags]
    public enum AudioSessionSetActiveOptions
    {
        None = 0,
        NotifyOthersOnDeactivation = 1 << 0
    }
}
