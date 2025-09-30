using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace _Sample
{
    internal sealed class MicrophoneViewController : MonoBehaviour
    {
        [SerializeField] private SampleApplication parent;
        [SerializeField] private AudioSource micAudioSource;
        [SerializeField] private Dropdown microphonesDropdown;
        [SerializeField] private Button startMicrophone;
        [SerializeField] private Button endMicrophone;
        private bool _isRecording = false;

        private void Start()
        {
            SetupMicrophoneButtons();

            // マイクの権限を確認
            StartCoroutine(CheckMicrophonePermission());
        }

        private void Update()
        {
            UpdateMicrophoneButtonStates();
        }

        private void SetupMicrophoneButtons()
        {
            Assert.IsTrue(micAudioSource != null);
            Assert.IsTrue(microphonesDropdown != null);
            Assert.IsTrue(startMicrophone != null);
            Assert.IsTrue(endMicrophone != null);

            startMicrophone.onClick.AddListener(StartMicrophone);
            endMicrophone.onClick.AddListener(StopMicrophone);

            // 初期状態の設定
            UpdateMicrophoneButtonStates();
        }

        private IEnumerator CheckMicrophonePermission()
        {
            // マイクの権限を確認
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                // 権限がない場合はリクエスト
                yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            }

            // 権限取得後にマイクリストを更新
            if (Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                PopulateMicrophoneDropdown();
                parent.UpdateTogglesState();
            }
            else
            {
                Debug.LogWarning("[SampleApplication] Microphone permission denied");
            }
        }

        private void PopulateMicrophoneDropdown()
        {
            microphonesDropdown.ClearOptions();

            var microphones = Microphone.devices;
            if (microphones.Length > 0)
            {
                var options = new List<string>(microphones);
                microphonesDropdown.AddOptions(options);
                microphonesDropdown.value = 0;
                microphonesDropdown.RefreshShownValue();

                Debug.Log($"[SampleApplication] Found {microphones.Length} microphone(s)");
            }
            else
            {
                Debug.LogWarning("[SampleApplication] No microphones found");
                microphonesDropdown.AddOptions(new List<string> { "No microphones available" });
            }
        }

        private void StartMicrophone()
        {
            if (_isRecording) return;

            var devices = Microphone.devices;
            if (devices.Length == 0)
            {
                Debug.LogWarning("[SampleApplication] No microphones available");
                return;
            }

            if (microphonesDropdown.value >= devices.Length)
            {
                Debug.LogWarning("[SampleApplication] Selected microphone index out of range");
                return;
            }

            var selectedDevice = devices[microphonesDropdown.value];

            // マイク録音を開始（44100Hz、1秒のループバッファ）
            var clip = Microphone.Start(selectedDevice, true, 1, 44100);

            if (clip != null)
            {
                micAudioSource.clip = clip;
                // マイクからの音声をリアルタイムで再生
                micAudioSource.loop = true;
                // マイクの録音が開始されるまで待機
                while (!(Microphone.GetPosition(selectedDevice) > 0))
                {
                }

                micAudioSource.Play();

                _isRecording = true;
                parent.UpdateTogglesState();
                Debug.Log($"[SampleApplication] Started recording from: {selectedDevice}");
            }
            else
            {
                Debug.LogError("[SampleApplication] Failed to start microphone");
            }
        }

        private void StopMicrophone()
        {
            if (!_isRecording) return;

            var devices = Microphone.devices;
            if (devices.Length > 0 && microphonesDropdown.value < devices.Length)
            {
                var selectedDevice = devices[microphonesDropdown.value];

                // マイク録音を停止
                if (Microphone.IsRecording(selectedDevice))
                {
                    Microphone.End(selectedDevice);
                }

                // オーディオ再生も停止
                if (micAudioSource.isPlaying)
                {
                    micAudioSource.Stop();
                }

                micAudioSource.clip = null;
                _isRecording = false;
                parent.UpdateTogglesState();
                Debug.Log($"[SampleApplication] Stopped recording from: {selectedDevice}");
            }
        }

        private void UpdateMicrophoneButtonStates()
        {
            startMicrophone.interactable = !_isRecording && Microphone.devices.Length > 0;
            endMicrophone.interactable = _isRecording;
        }
    }
}
