namespace AVAudioSessionBridge
{
    public interface IAudioSession
    {
        AudioSessionCategory Category { get; }
        AudioSessionCategoryOptions CategoryOptions { get; }
        AudioSessionMode Mode { get; }
        bool IsOtherAudioPlaying { get; }
        bool SecondaryAudioShouldBeSilencedHint { get; }

        bool SetCategory(AudioSessionCategory category, AudioSessionCategoryOptions options = AudioSessionCategoryOptions.None);
        bool SetMode(AudioSessionMode mode);
        bool SetActive(bool active, AudioSessionSetActiveOptions options = AudioSessionSetActiveOptions.None);
        void UnitySetAudioSessionActive(bool active);

        void PrintInfo();
        string GetInfoString(bool isDetail);
    }
}
