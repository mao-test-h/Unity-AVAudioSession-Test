# AVAudioSession-Test

こちらは [CA.unity #10](https://cyberagent.connpass.com/event/366674/) と言うイベントで講演した「iOSプラットフォーム向けの開発Tips」にある「オーディオの制御について」の章のサンプルプロジェクトです。

<img width="300" src="https://github.com/user-attachments/assets/9651dcc8-63f5-4fc5-83a2-605d6d0c3807" />


## 各項目について

- **BGM**
  - Play BGM
    - Audio Source を用いて BGM を再生
  - Stop BGM
    - 再生した BGM を停止
- **Microphone**
  - 入力選択用のドロップダウン
    - [`Microphone.devices`](https://docs.unity3d.com/ScriptReference/Microphone-devices.html) の項目を選択
  - Start Mic
    - [`Microphone.Start`](https://docs.unity3d.com/ScriptReference/Microphone.Start.html) の呼び出し
  - End Mic
    - [`Microphone.End`](https://docs.unity3d.com/ScriptReference/Microphone.End.html) の呼び出し
- **AudioSession**
  - Activate(true), Activate(false)
    - [`AVAudioSession.sharedInstance().setActive()`](https://developer.apple.com/documentation/avfaudio/avaudiosession/setactive(_:options:)) の呼び出し
      - ※ true, false ともに Options には `.notifyOthersOnDeactivation` を渡してます 
  - UnityActive(true), UnityActive(false)
    - `UnitySetAudioSessionActive()` の呼び出し
  - Print
    - 現在の `AVAudioSession` の状態をログ出力
  - Reset
    - [`AudioSettings.Reset`](https://docs.unity3d.com/ScriptReference/AudioSettings.Reset.html) の呼び出し 
  - Show Detail
    - ON 時には後述の `AudioSession Info` 欄に詳細を出力

<img width="300" src="https://github.com/user-attachments/assets/40e4600c-05d9-429b-9b8c-d3a8042cd645" />

### AudioSession Info

AVAudioSession の各種パラメータの状態を出力します。  
詳細まで表示したい際には `Show Detail` を ON にしてください。

<img width="300" src="https://github.com/user-attachments/assets/7ba10233-352c-4d0d-ab87-d860da9d416e" />

### Category, Mode, CategoryOption

AVAudioSession にある Category, Mode, CategoryOption を変更します。

> [!warning]
> 現時点の実装だと、AVAudioSession 側でエラーが発生すると Unity の UI に反映されなかったりします。。  
> 発生時には Xcode 側でエラーログを出すようにしているので、そちらをご確認ください。
> (Unity のログには出ないので注意)  

<img width="300" src="https://github.com/user-attachments/assets/d58f321a-c5c5-4e41-9333-93ae3817d1ce" />


# License

## BGM/SE

- [魔王魂](https://maoudamashii.jokersounds.com/)
# AVAudioSession-Test
