using System;
using System.Collections.Generic;
using AVAudioSessionBridge;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace _Sample
{
    internal sealed class SampleApplication : MonoBehaviour
    {
        [SerializeField] private AudioSource bgmAudioSource;
        [SerializeField] private Button playBgmButton;
        [SerializeField] private Button stopBgmButton;

        [SerializeField] private Button activateButton;
        [SerializeField] private Button deactivateButton;
        [SerializeField] private Button unityActivateButton;
        [SerializeField] private Button unityDeactivateButton;
        [SerializeField] private Button printInfoButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Toggle isShowDetailToggle;

        [SerializeField] private Toggle categoryTogglePrefab;
        [SerializeField] private Toggle modeTogglePrefab;
        [SerializeField] private Toggle optionsTogglePrefab;
        [SerializeField] private Text audioSessionInfoText;

        private IAudioSession _audioSession;
        private readonly List<Toggle> _categoryToggles = new();
        private readonly List<Toggle> _modeToggles = new();
        private readonly List<Toggle> _optionsToggles = new();

        private void Start()
        {
            _audioSession = AudioSessionFactory.Create();

            SetupBgmButtons();
            SetupActivationButtons();
            SetupCategoryToggles();
            SetupModeToggles();
            SetupOptionsToggles();

            UpdateStatusTexts();
            UpdateTogglesState();
        }

        private void Update()
        {
            UpdateStatusTexts();
        }

        private void SetupBgmButtons()
        {
            Assert.IsTrue(bgmAudioSource != null);
            playBgmButton.onClick.AddListener(() =>
            {
                if (!bgmAudioSource.isPlaying)
                {
                    bgmAudioSource.Play();
                }
            });

            stopBgmButton.onClick.AddListener(() =>
            {
                if (bgmAudioSource.isPlaying)
                {
                    bgmAudioSource.Stop();
                }
            });
        }

        private void SetupActivationButtons()
        {
            // TODO: 各種ボタン実行時にトグルのステータスを更新する

            activateButton.onClick.AddListener(() =>
            {
                _audioSession.SetActive(true, AudioSessionSetActiveOptions.NotifyOthersOnDeactivation);
                UpdateTogglesState();
            });

            deactivateButton.onClick.AddListener(() =>
            {
                _audioSession.SetActive(false, AudioSessionSetActiveOptions.NotifyOthersOnDeactivation);
                UpdateTogglesState();
            });

            unityActivateButton.onClick.AddListener(() =>
            {
                _audioSession.UnitySetAudioSessionActive(true);
                UpdateTogglesState();
            });

            unityDeactivateButton.onClick.AddListener(() =>
            {
                _audioSession.UnitySetAudioSessionActive(false);
                UpdateTogglesState();
            });

            printInfoButton.onClick.AddListener(() =>
            {
                _audioSession.PrintInfo();
            });

            resetButton.onClick.AddListener(() =>
            {
                var config = AudioSettings.GetConfiguration();
                AudioSettings.Reset(config);

                // Reset 実行後にトグルの状態を更新
                UpdateTogglesState();
            });
        }

        private void SetupCategoryToggles()
        {
            var parent = categoryTogglePrefab.transform.parent;
            var currentCategory = _audioSession.Category;

            foreach (AudioSessionCategory category in Enum.GetValues(typeof(AudioSessionCategory)))
            {
                var toggle = Instantiate(categoryTogglePrefab, parent);
                toggle.GetComponentInChildren<Text>().text = category.ToString();
                toggle.isOn = category == currentCategory;
                toggle.gameObject.SetActive(true);

                var capturedCategory = category;
                toggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        _audioSession.SetCategory(capturedCategory);
                        UpdateOtherCategoryToggles(toggle);
                    }
                });

                _categoryToggles.Add(toggle);
            }

            categoryTogglePrefab.gameObject.SetActive(false);
        }

        private void SetupModeToggles()
        {
            var parent = modeTogglePrefab.transform.parent;
            var currentMode = _audioSession.Mode;

            foreach (AudioSessionMode mode in Enum.GetValues(typeof(AudioSessionMode)))
            {
                var toggle = Instantiate(modeTogglePrefab, parent);
                toggle.GetComponentInChildren<Text>().text = mode.ToString();
                toggle.isOn = mode == currentMode;
                toggle.gameObject.SetActive(true);

                var capturedMode = mode;
                toggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        _audioSession.SetMode(capturedMode);
                        UpdateOtherModeToggles(toggle);
                    }
                });

                _modeToggles.Add(toggle);
            }

            modeTogglePrefab.gameObject.SetActive(false);
        }

        private void SetupOptionsToggles()
        {
            var parent = optionsTogglePrefab.transform.parent;

            foreach (AudioSessionCategoryOptions option in Enum.GetValues(typeof(AudioSessionCategoryOptions)))
            {
                if (option == AudioSessionCategoryOptions.None) continue;

                var toggle = Instantiate(optionsTogglePrefab, parent);
                toggle.GetComponentInChildren<Text>().text = option.ToString();
                toggle.isOn = false;
                toggle.gameObject.SetActive(true);

                toggle.onValueChanged.AddListener(_ => ApplyOptions());
                _optionsToggles.Add(toggle);
            }

            optionsTogglePrefab.gameObject.SetActive(false);
        }

        private void ApplyOptions()
        {
            var options = AudioSessionCategoryOptions.None;
            var values = Enum.GetValues(typeof(AudioSessionCategoryOptions));

            for (int i = 0; i < _optionsToggles.Count; i++)
            {
                if (_optionsToggles[i].isOn)
                {
                    options |= (AudioSessionCategoryOptions)values.GetValue(i + 1);
                }
            }

            var currentCategory = _audioSession.Category;
            _audioSession.SetCategory(currentCategory, options);
        }

        private void UpdateOtherCategoryToggles(Toggle activeToggle)
        {
            foreach (var toggle in _categoryToggles)
            {
                if (toggle != activeToggle)
                {
                    toggle.SetIsOnWithoutNotify(false);
                }
            }
        }

        private void UpdateOtherModeToggles(Toggle activeToggle)
        {
            foreach (var toggle in _modeToggles)
            {
                if (toggle != activeToggle)
                {
                    toggle.SetIsOnWithoutNotify(false);
                }
            }
        }

        private void UpdateStatusTexts()
        {
            var isDetail = isShowDetailToggle.isOn;
            audioSessionInfoText.text = $"{_audioSession.GetInfoString(isDetail) ?? "Null"}";
        }

        internal void UpdateTogglesState()
        {
            // カテゴリトグルの状態を更新
            var currentCategory = _audioSession.Category;
            for (int i = 0; i < _categoryToggles.Count; i++)
            {
                var category = (AudioSessionCategory)Enum.GetValues(typeof(AudioSessionCategory)).GetValue(i);
                _categoryToggles[i].SetIsOnWithoutNotify(category == currentCategory);
            }

            // モードトグルの状態を更新
            var currentMode = _audioSession.Mode;
            for (int i = 0; i < _modeToggles.Count; i++)
            {
                var mode = (AudioSessionMode)Enum.GetValues(typeof(AudioSessionMode)).GetValue(i);
                _modeToggles[i].SetIsOnWithoutNotify(mode == currentMode);
            }

            // オプショントグルをネイティブ側の値で更新
            var currentOptions = _audioSession.CategoryOptions;
            var optionValues = Enum.GetValues(typeof(AudioSessionCategoryOptions));
            for (int i = 0; i < _optionsToggles.Count; i++)
            {
                var option = (AudioSessionCategoryOptions)optionValues.GetValue(i + 1); // None をスキップ
                _optionsToggles[i].SetIsOnWithoutNotify((currentOptions & option) != 0);
            }
        }
    }
}
