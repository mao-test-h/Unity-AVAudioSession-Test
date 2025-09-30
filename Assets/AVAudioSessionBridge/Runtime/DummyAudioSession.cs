#if !UNITY_IOS || UNITY_EDITOR
using UnityEngine;

namespace AVAudioSessionBridge
{
    internal sealed class DummyAudioSession : IAudioSession
    {
        public AudioSessionCategory Category => AudioSessionCategory.SoloAmbient;

        public AudioSessionCategoryOptions CategoryOptions => AudioSessionCategoryOptions.None;

        public AudioSessionMode Mode => AudioSessionMode.Default;

        public bool IsOtherAudioPlaying => false;

        public bool SecondaryAudioShouldBeSilencedHint => false;

        public bool SetCategory(AudioSessionCategory category, AudioSessionCategoryOptions options = AudioSessionCategoryOptions.None)
        {
            Debug.Log($"[DummyAudioSession] SetCategory: {category}, options: {options}");
            return true;
        }

        public bool SetMode(AudioSessionMode mode)
        {
            Debug.Log($"[DummyAudioSession] SetMode: {mode}");
            return true;
        }

        public bool SetActive(bool active, AudioSessionSetActiveOptions options = AudioSessionSetActiveOptions.None)
        {
            Debug.Log($"[DummyAudioSession] SetActive: {active}, options: {options}");
            return true;
        }

        public void UnitySetAudioSessionActive(bool active)
        {
        }

        public void PrintInfo()
        {
        }

        public string GetInfoString(bool isDetail)
        {
            return string.Empty;
        }
    }
}
#endif
