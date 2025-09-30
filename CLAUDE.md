# CLAUDE.md

## プロジェクト概要

これは iOS の AVAudioSession テスト用の Unity プロジェクトです。  
Unity 6000 (LTS) を使用しており、Universal Render Pipeline (URP) が設定されています。  

### 開発環境

- Unity 6000.0.57f1
  - JetBrains Rider
  - VSCode
- Xcode 16.4

### プロジェクト構造

- **Assets/_Sample/** : サンプルのプロジェクトコード
  - **Runtime/** : 実行時コード (.cs)
  - **Scenes/** : シーンファイル
  - **Settings/** : Input Systemなどの設定ファイル
- **Assets/AVAudioSessionBridge/**   : AVAudioSession を実行するためのブリッジ
  - **Plugins**
    - **iOS** : iOS のネイティブプラグイン (.swift)
  - **Runtime/** : P/Invoke (.cs)
- **Assets/AVFoundationBridge/**   : AVFoundation を実行するためのブリッジ
  - **Plugins**
    - **iOS** : iOS のネイティブプラグイン (.swift)
  - **Runtime/** : P/Invoke (.cs)


#### 依存関係
主要な依存パッケージ：
- com.unity.render-pipelines.universal: 17.0.4
- com.unity.inputsystem: 1.14.2
- com.unity.modules.audio: 1.0.0（オーディオ関連）


------------------------------

# ネイティブプラグイン開発

## プラグインの実装と呼び出し方

- iOS 向けのネイティブプラグインは Swift で実装すること
  - P/Invoke 向けの C関数の外部宣言は `@_cdecl` を用いること
  - 以下に実装例を示す

```swift
import Foundation

// MARK: - Hello World

// ネイティブで `Hello World` を出力するサンプル
// NOTE: `@_cdecl` を用いて C の関数として公開 (パラメータには公開する際の関数名を渡す)
@_cdecl("printHelloWorld")
public func printHelloWorld() {
    // このメソッドが C# から P/Invoke 経由で呼び出される
    print("[Swift]: Hello World")
}

// MARK: - 引数と戻り値の受け渡しを行うサンプル

// 引数と戻り値の受け渡しを行うサンプル (整数)
// NOTE: C# とネイティブ側で型は合わせること (例えば Swift の Int は環境によって 32bit / 64bit が変わるので明示的にサイズを指定する)
@_cdecl("paramSampleInt")
public func paramSampleInt(_ value: Int32) -> Int32 {
    print("[Swift]: \(value)")
    return 2
}

// 引数と戻り値の受け渡しを行うサンプル (文字列)
@_cdecl("paramSampleString")
public func paramSampleString(_ strPtr: UnsafePointer<CChar>?) -> UnsafePointer<CChar>? {
    
    if let strPtr = strPtr {
        let message = String(cString: strPtr)
        print("[Swift]: \(message)")
    }
    
    // 戻り値として送りたい文字列
    let message = "Swift Message"
    
    let utfText: UnsafePointer<CChar>? = (message as NSString).utf8String;
    let pointer: UnsafeMutablePointer<Int8> = UnsafeMutablePointer<Int8>.allocate(capacity: (8 * message.count) + 1);
    return UnsafePointer(strcpy(pointer, utfText))
}
```

```csharp
public void PrintHelloWorld()
{
    NativeMethod();

    [DllImport("__Internal", EntryPoint = "printHelloWorld")]
    static extern void NativeMethod();
}


public Int32 ParamSample(Int32 value)
{
    return NativeMethod(value);

    [DllImport("__Internal", EntryPoint = "paramSampleInt")]
    static extern Int32 NativeMethod(Int32 value);
}

public string ParamSample(string message)
{
    return NativeMethod(message);

    [DllImport("__Internal", EntryPoint = "paramSampleString")]
    static extern string NativeMethod(string message);
}
```

### インスタンスメソッドの呼び出し

```swift
// MARK: - インスタンスメソッドの呼び出し

class SampleClass {
    func SampleMethod() {
        print("[Swift]: Call SampleMethod")
    }
}

// インスタンスの生成
@_cdecl("createInstance")
public func createInstance() -> UnsafeMutableRawPointer? {
    let instance = SampleClass()
    let unmanaged = Unmanaged<SampleClass>.passRetained(instance)
    return unmanaged.toOpaque()
}

// インスタンスメソッドの呼び出し
@_cdecl("callInstanceMethod")
public func callInstanceMethod(_ instancePtr: UnsafeRawPointer) {
    let instance = Unmanaged<SampleClass>.fromOpaque(instancePtr).takeUnretainedValue()
    instance.SampleMethod()
}

// インスタンスの解放
@_cdecl("releaseInstance")
public func releaseInstance(_ instancePtr: UnsafeRawPointer) {
    let unmanaged = Unmanaged<SampleClass>.fromOpaque(instancePtr)
    unmanaged.release()
}
```

```csharp
private IntPtr _instance = IntPtr.Zero;

public void CreateInstance()
{
    if (_instance != IntPtr.Zero)
    {
        Debug.Log("[Unity]: Instance already created");
        return;
    }

    _instance = NativeMethod();
    Debug.Log("[Unity]: Instance created");

    [DllImport("__Internal", EntryPoint = "createInstance")]
    static extern IntPtr NativeMethod();
}

public void CallInstanceMethod()
{
    if (_instance == IntPtr.Zero)
    {
        Debug.Log("[Unity]: Instance not created");
        return;
    }

    NativeMethod(_instance);

    [DllImport("__Internal", EntryPoint = "callInstanceMethod")]
    static extern void NativeMethod(IntPtr instance);
}

public void ReleaseInstance()
{
    if (_instance == IntPtr.Zero)
    {
        Debug.Log("[Unity]: Instance not created");
        return;
    }

    NativeMethod(_instance);
    _instance = IntPtr.Zero;
    Debug.Log("[Unity]: Instance released");

    [DllImport("__Internal", EntryPoint = "releaseInstance")]
    static extern void NativeMethod(IntPtr instance);
}
```

### コールバックの呼び出し

```swift
// MARK: - コールバックの呼び出し

public typealias SampleCallback = @convention(c) (UnsafePointer<CChar>?) -> Void

@_cdecl("callbackSample")
func callbackSample(_ sampleCallback: SampleCallback) {
    let message = "Swift Callback Message"   // 戻り値として送りたい文字列
    let utfText: UnsafePointer<CChar>? = (message as NSString).utf8String;
    sampleCallback(utfText)
}
```

```csharp
private delegate void SampleCallback(string message);

public void CallbackSample()
{
    NativeMethod(OnCallback);

    // iOS (正確に言うと IL2CPP) の場合には Static Method に対し、`MonoPInvokeCallbackAttribute` を付ける必要がある
    [MonoPInvokeCallback(typeof(SampleCallback))]
    static void OnCallback(string message)
    {
        // このメソッドがネイティブから呼び出される
        Debug.Log("[Unity]: " + message);
    }

    [DllImport("__Internal", EntryPoint = "callbackSample")]
    static extern void NativeMethod([MarshalAs(UnmanagedType.FunctionPtr)] SampleCallback sampleCallback);
}
```
