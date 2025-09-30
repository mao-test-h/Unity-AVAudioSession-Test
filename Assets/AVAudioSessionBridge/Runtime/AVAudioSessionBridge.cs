#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AVAudioSessionBridge
{
    internal sealed class AVAudioSessionBridge : IAudioSession
    {
        public AudioSessionCategory Category => ParseCategory(GetCategory());
        public AudioSessionCategoryOptions CategoryOptions => (AudioSessionCategoryOptions)GetCategoryOptions();
        public AudioSessionMode Mode => ParseMode(GetMode());
        public bool IsOtherAudioPlaying => GetIsOtherAudioPlaying() == 1;
        public bool SecondaryAudioShouldBeSilencedHint => GetSecondaryAudioShouldBeSilencedHint() == 1;

        public bool SetCategory(AudioSessionCategory category, AudioSessionCategoryOptions options = AudioSessionCategoryOptions.None)
        {
            var categoryStr = GetCategoryString(category);
            var optionsStr = GetCategoryOptionsString(options);
            return SetCategoryNative(categoryStr, optionsStr) == 1;
        }

        public bool SetMode(AudioSessionMode mode)
        {
            var modeStr = GetModeString(mode);
            return SetModeNative(modeStr) == 1;
        }

        public bool SetActive(bool active, AudioSessionSetActiveOptions options = AudioSessionSetActiveOptions.None)
        {
            var optionsStr = GetSetActiveOptionsString(options);
            return SetActiveNative((byte)(active ? 1 : 0), optionsStr) == 1;
        }

        public void UnitySetAudioSessionActive(bool active)
        {
            UnitySetAudioSessionActiveNative(active ? 1 : 0);
        }

        public void PrintInfo()
        {
            PrintAudioSessionInfo();
        }

        public string GetInfoString(bool isDetail)
        {
            return GetAudioSessionInfo((byte)(isDetail ? 1 : 0));
        }

        [DllImport("__Internal", EntryPoint = "AVAudioSessionBridge_GetCategory")]
        private static extern string GetCategory();

        [DllImport("__Internal", EntryPoint = "AVAudioSessionBridge_GetMode")]
        private static extern string GetMode();

        [DllImport("__Internal", EntryPoint = "AVAudioSessionBridge_GetCategoryOptions")]
        private static extern uint GetCategoryOptions();

        [DllImport("__Internal", EntryPoint = "AVAudioSessionBridge_GetIsOtherAudioPlaying")]
        private static extern byte GetIsOtherAudioPlaying();

        [DllImport("__Internal", EntryPoint = "AVAudioSessionBridge_GetSecondaryAudioShouldBeSilencedHint")]
        private static extern byte GetSecondaryAudioShouldBeSilencedHint();

        [DllImport("__Internal", EntryPoint = "AVAudioSessionBridge_SetCategory")]
        private static extern byte SetCategoryNative(string category, string options);

        [DllImport("__Internal", EntryPoint = "AVAudioSessionBridge_SetMode")]
        private static extern byte SetModeNative(string mode);

        [DllImport("__Internal", EntryPoint = "AVAudioSessionBridge_SetActive")]
        private static extern byte SetActiveNative(byte active, string options);

        [DllImport("__Internal", EntryPoint = "AVAudioSessionBridge_UnitySetAudioSessionActive")]
        private static extern byte UnitySetAudioSessionActiveNative(int active);

        [DllImport("__Internal", EntryPoint = "AVAudioSessionBridge_PrintAudioSessionInfo")]
        private static extern void PrintAudioSessionInfo();

        [DllImport("__Internal", EntryPoint = "AVAudioSessionBridge_GetAudioSessionInfo")]
        private static extern string GetAudioSessionInfo(byte isDetail);


        private static AudioSessionCategory ParseCategory(string category)
        {
            return category switch
            {
                "AVAudioSessionCategoryAmbient" => AudioSessionCategory.Ambient,
                "AVAudioSessionCategorySoloAmbient" => AudioSessionCategory.SoloAmbient,
                "AVAudioSessionCategoryPlayback" => AudioSessionCategory.Playback,
                "AVAudioSessionCategoryRecord" => AudioSessionCategory.Record,
                "AVAudioSessionCategoryPlayAndRecord" => AudioSessionCategory.PlayAndRecord,
                "AVAudioSessionCategoryMultiRoute" => AudioSessionCategory.MultiRoute,
                _ => throw new ArgumentException()
            };
        }

        private static AudioSessionMode ParseMode(string mode)
        {
            return mode switch
            {
                "AVAudioSessionModeDefault" => AudioSessionMode.Default,
                "AVAudioSessionModeVoiceChat" => AudioSessionMode.VoiceChat,
                "AVAudioSessionModeGameChat" => AudioSessionMode.GameChat,
                "AVAudioSessionModeVideoRecording" => AudioSessionMode.VideoRecording,
                "AVAudioSessionModeMeasurement" => AudioSessionMode.Measurement,
                "AVAudioSessionModeMoviePlayback" => AudioSessionMode.MoviePlayback,
                "AVAudioSessionModeVideoChat" => AudioSessionMode.VideoChat,
                "AVAudioSessionModeSpokenAudio" => AudioSessionMode.SpokenAudio,
                "AVAudioSessionModeVoicePrompt" => AudioSessionMode.VoicePrompt,
                _ => throw new ArgumentException()
            };
        }

        private static string GetCategoryString(AudioSessionCategory category)
        {
            return category switch
            {
                AudioSessionCategory.Ambient => "AVAudioSessionCategoryAmbient",
                AudioSessionCategory.SoloAmbient => "AVAudioSessionCategorySoloAmbient",
                AudioSessionCategory.Playback => "AVAudioSessionCategoryPlayback",
                AudioSessionCategory.Record => "AVAudioSessionCategoryRecord",
                AudioSessionCategory.PlayAndRecord => "AVAudioSessionCategoryPlayAndRecord",
                AudioSessionCategory.MultiRoute => "AVAudioSessionCategoryMultiRoute",
                _ => throw new ArgumentException()
            };
        }

        private static string GetModeString(AudioSessionMode mode)
        {
            return mode switch
            {
                AudioSessionMode.Default => "AVAudioSessionModeDefault",
                AudioSessionMode.VoiceChat => "AVAudioSessionModeVoiceChat",
                AudioSessionMode.GameChat => "AVAudioSessionModeGameChat",
                AudioSessionMode.VideoRecording => "AVAudioSessionModeVideoRecording",
                AudioSessionMode.Measurement => "AVAudioSessionModeMeasurement",
                AudioSessionMode.MoviePlayback => "AVAudioSessionModeMoviePlayback",
                AudioSessionMode.VideoChat => "AVAudioSessionModeVideoChat",
                AudioSessionMode.SpokenAudio => "AVAudioSessionModeSpokenAudio",
                AudioSessionMode.VoicePrompt => "AVAudioSessionModeVoicePrompt",
                _ => throw new ArgumentException()
            };
        }

        private static string GetCategoryOptionsString(AudioSessionCategoryOptions options)
        {
            if (options == AudioSessionCategoryOptions.None)
            {
                return string.Empty;
            }

            var optionsList = new List<string>();
            if ((options & AudioSessionCategoryOptions.MixWithOthers) != 0)
            {
                optionsList.Add("mixWithOthers");
            }

            if ((options & AudioSessionCategoryOptions.DuckOthers) != 0)
            {
                optionsList.Add("duckOthers");
            }

            if ((options & AudioSessionCategoryOptions.AllowBluetooth) != 0)
            {
                optionsList.Add("allowBluetooth");
            }

            if ((options & AudioSessionCategoryOptions.DefaultToSpeaker) != 0)
            {
                optionsList.Add("defaultToSpeaker");
            }

            if ((options & AudioSessionCategoryOptions.InterruptSpokenAudioAndMixWithOthers) != 0)
            {
                optionsList.Add("interruptSpokenAudioAndMixWithOthers");
            }

            if ((options & AudioSessionCategoryOptions.AllowBluetoothA2DP) != 0)
            {
                optionsList.Add("allowBluetoothA2DP");
            }

            if ((options & AudioSessionCategoryOptions.AllowAirPlay) != 0)
            {
                optionsList.Add("allowAirPlay");
            }

            if ((options & AudioSessionCategoryOptions.OverrideMutedMicrophoneInterruption) != 0)
            {
                optionsList.Add("overrideMutedMicrophoneInterruption");
            }

            return string.Join(",", optionsList);
        }

        private static string GetSetActiveOptionsString(AudioSessionSetActiveOptions options)
        {
            if (options == AudioSessionSetActiveOptions.None)
            {
                return string.Empty;
            }

            if ((options & AudioSessionSetActiveOptions.NotifyOthersOnDeactivation) != 0)
            {
                return "notifyOthersOnDeactivation";
            }

            return string.Empty;
        }
    }
}
#endif
