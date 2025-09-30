import Foundation
import AVFoundation
import OSLog

let logger = Logger(subsystem: Bundle.main.bundleIdentifier!, category: "ApplicationCode")

@_cdecl("AVAudioSessionBridge_GetCategory")
public func AVAudioSessionBridge_GetCategory() -> UnsafeMutablePointer<CChar>? {
    let category = AVAudioSession.sharedInstance().category.rawValue
    let utfText: UnsafePointer<CChar>? = (category as NSString).utf8String
    let pointer: UnsafeMutablePointer<Int8> = UnsafeMutablePointer<Int8>.allocate(capacity: (8 * category.count) + 1)
    return strcpy(pointer, utfText)
}

@_cdecl("AVAudioSessionBridge_GetMode")
public func AVAudioSessionBridge_GetMode() -> UnsafeMutablePointer<CChar>? {
    let mode = AVAudioSession.sharedInstance().mode.rawValue
    let utfText: UnsafePointer<CChar>? = (mode as NSString).utf8String
    let pointer: UnsafeMutablePointer<Int8> = UnsafeMutablePointer<Int8>.allocate(capacity: (8 * mode.count) + 1)
    return strcpy(pointer, utfText)
}

@_cdecl("AVAudioSessionBridge_GetCategoryOptions")
public func AVAudioSessionBridge_GetCategoryOptions() -> UInt32 {
    let options = AVAudioSession.sharedInstance().categoryOptions
    return UInt32(options.rawValue)
}

@_cdecl("AVAudioSessionBridge_GetIsOtherAudioPlaying")
public func AVAudioSessionBridge_GetIsOtherAudioPlaying() -> UInt8 {
    return AVAudioSession.sharedInstance().isOtherAudioPlaying ? 1 : 0
}

@_cdecl("AVAudioSessionBridge_GetSecondaryAudioShouldBeSilencedHint")
public func AVAudioSessionBridge_GetSecondaryAudioShouldBeSilencedHint() -> UInt8 {
    return AVAudioSession.sharedInstance().secondaryAudioShouldBeSilencedHint ? 1 : 0
}

@_cdecl("AVAudioSessionBridge_SetCategory")
public func AVAudioSessionBridge_SetCategory(_ categoryPtr: UnsafePointer<CChar>?, _ optionsPtr: UnsafePointer<CChar>?) -> UInt8 {
    guard let categoryPtr = categoryPtr else {
        return 0
    }
    
    let categoryString = String(cString: categoryPtr)
    let category = AVAudioSession.Category(rawValue: categoryString)
    
    var options: AVAudioSession.CategoryOptions = []
    if let optionsPtr = optionsPtr {
        let optionsString = String(cString: optionsPtr)
        let optionsList = optionsString.split(separator: ",")
        
        for option in optionsList {
            switch option.trimmingCharacters(in: .whitespaces) {
            case "mixWithOthers":
                options.insert(.mixWithOthers)
            case "duckOthers":
                options.insert(.duckOthers)
            case "allowBluetooth":
                options.insert(.allowBluetooth)
            case "defaultToSpeaker":
                options.insert(.defaultToSpeaker)
            case "interruptSpokenAudioAndMixWithOthers":
                options.insert(.interruptSpokenAudioAndMixWithOthers)
            case "allowBluetoothA2DP":
                options.insert(.allowBluetoothA2DP)
            case "allowAirPlay":
                options.insert(.allowAirPlay)
            case "overrideMutedMicrophoneInterruption":
                if #available(iOS 14.5, *) {
                    options.insert(.overrideMutedMicrophoneInterruption)
                }
            default:
                logger.error("[Swift]: Unknown option: \(option)")
            }
        }
    }
    
    do {
        try AVAudioSession.sharedInstance().setCategory(category, options: options)
        return 1
    } catch {
        logger.error("[Swift]: Failed to set category: \(error)")
        return 0
    }
}

@_cdecl("AVAudioSessionBridge_SetMode")
public func AVAudioSessionBridge_SetMode(_ modePtr: UnsafePointer<CChar>?) -> UInt8 {
    guard let modePtr = modePtr else {
        return 0
    }
    
    let modeString = String(cString: modePtr)
    let mode = AVAudioSession.Mode(rawValue: modeString)
    
    do {
        try AVAudioSession.sharedInstance().setMode(mode)
        return 1
    } catch {
        logger.error("[Swift]: Failed to set mode: \(error)")
        return 0
    }
}

@_cdecl("AVAudioSessionBridge_SetActive")
public func AVAudioSessionBridge_SetActive(_ active: UInt8, _ optionsPtr: UnsafePointer<CChar>?) -> UInt8 {
    var options: AVAudioSession.SetActiveOptions = []
    if let optionsPtr = optionsPtr {
        let optionsString = String(cString: optionsPtr)
        if optionsString == "notifyOthersOnDeactivation" {
            options = .notifyOthersOnDeactivation
        }
    }
    
    do {
        if active == 1 {
            try AVAudioSession.sharedInstance().setActive(true, options: options)
        } else {
            try AVAudioSession.sharedInstance().setActive(false, options: options)
        }
        return 1
    } catch {
        logger.error("[Swift]: Failed to set active: \(error)")
        return 0
    }
}

@_cdecl("AVAudioSessionBridge_GetOutputVolume")
public func AVAudioSessionBridge_GetOutputVolume() -> Float {
    return AVAudioSession.sharedInstance().outputVolume
}

@_cdecl("AVAudioSessionBridge_PrintAudioSessionInfo")
public func AVAudioSessionBridge_PrintAudioSessionInfo() {
    let info = AVAudioSessionBridge_GetAudioSessionInfo(1)
    let infoString = String(cString: info!)
    
    let categoryOptionsInfo = """
    
    CategoryOptions:
        - mixWithOthers: \(AVAudioSession.CategoryOptions.mixWithOthers)
        - duckOthers: \(AVAudioSession.CategoryOptions.duckOthers)
        - allowBluetooth: \(AVAudioSession.CategoryOptions.allowBluetooth)
        - defaultToSpeaker: \(AVAudioSession.CategoryOptions.defaultToSpeaker)
        - interruptSpokenAudioAndMixWithOthers: \(AVAudioSession.CategoryOptions.interruptSpokenAudioAndMixWithOthers)
        - allowBluetoothA2DP: \(AVAudioSession.CategoryOptions.allowBluetoothA2DP)
        - allowAirPlay: \(AVAudioSession.CategoryOptions.allowAirPlay)
    """
    
    logger.info("\(infoString)\(categoryOptionsInfo)")
}

@_cdecl("AVAudioSessionBridge_GetAudioSessionInfo")
public func AVAudioSessionBridge_GetAudioSessionInfo(_ isDetail: UInt8) -> UnsafeMutablePointer<CChar>? {
    let session = AVAudioSession.sharedInstance()
    let isDetail = isDetail == 1
    
    var category: [String: Any] = [:]
    category["category"] = session.category.rawValue
    category["availableCategories"] = session.availableCategories.map { $0.rawValue }
    
    
    var mode: [String: Any] = [:]
    mode["mode"] = session.mode.rawValue
    mode["availableModes"] = session.availableModes.map { $0.rawValue }
    
    
    var categoryOptions: [String: Any] = [:]
    categoryOptions["rawValue"] = session.categoryOptions.rawValue
    categoryOptions["mixWithOthers"] = session.categoryOptions.contains(.mixWithOthers)
    categoryOptions["duckOthers"] = session.categoryOptions.contains(.duckOthers)
    categoryOptions["allowBluetooth"] = session.categoryOptions.contains(.allowBluetooth)
    categoryOptions["defaultToSpeaker"] = session.categoryOptions.contains(.defaultToSpeaker)
    categoryOptions["interruptSpokenAudioAndMixWithOthers"] = session.categoryOptions.contains(.interruptSpokenAudioAndMixWithOthers)
    categoryOptions["allowBluetoothA2DP"] = session.categoryOptions.contains(.allowBluetoothA2DP)
    categoryOptions["allowAirPlay"] = session.categoryOptions.contains(.allowAirPlay)
    if #available(iOS 14.5, *) {
        categoryOptions["[iOS 14.5+] overrideMutedMicrophoneInterruption"] = session.categoryOptions.contains(.overrideMutedMicrophoneInterruption)
    }
    
    
    var renderingMode: [String: Any] = [:]
    if #available(iOS 17.2, *) {
        renderingMode["[iOS 17.2+] rawValue"] = session.renderingMode.rawValue
    }
    
    
    var flags: [String: Bool] = [:]
    flags["isOtherAudioPlaying"] = session.isOtherAudioPlaying
    flags["secondaryAudioShouldBeSilencedHint"] = session.secondaryAudioShouldBeSilencedHint
    flags["allowHapticsAndSystemSoundsDuringRecording"] = session.allowHapticsAndSystemSoundsDuringRecording
    if #available(iOS 14.5, *) {
        flags["[iOS 14.5+] prefersNoInterruptionsFromSystemAlerts"] = session.prefersNoInterruptionsFromSystemAlerts
    }
    if #available(iOS 17.0, *) {
        flags["[iOS 17.0+] prefersInterruptionOnRouteDisconnect"] = session.prefersInterruptionOnRouteDisconnect
    }
    
    
    var currentRoute: [String: Any] = [:]
    var inputs: [[String: String]] = []
    for input in session.currentRoute.inputs {
        inputs.append([
            "portName": input.portName,
            "portType": input.portType.rawValue
        ])
    }
    currentRoute["inputs"] = inputs
    var outputs: [[String: String]] = []
    for output in session.currentRoute.outputs {
        outputs.append([
            "portName": output.portName,
            "portType": output.portType.rawValue
        ])
    }
    currentRoute["outputs"] = outputs
    
    
    var hardware: [String: Any] = [:]
    var sampleRate: [String: Any] = [:]
    sampleRate["sampleRate"] = session.sampleRate
    sampleRate["preferredSampleRate"] = session.preferredSampleRate
    hardware["sampleRate"] = sampleRate
    
    var inputGain: [String: Any] = [:]
    inputGain["inputGain"] = session.inputGain
    inputGain["isInputGainSettable"] = session.isInputGainSettable
    hardware["inputGain"] = inputGain
    
    var IOBufferDuration: [String: Any] = [:]
    IOBufferDuration["ioBufferDuration"] = session.ioBufferDuration
    IOBufferDuration["preferredIOBufferDuration"] = session.preferredIOBufferDuration
    hardware["IOBufferDuration"] = IOBufferDuration
    
    var inspectingLatency: [String: Any] = [:]
    inspectingLatency["inputLatency"] = session.inputLatency
    inspectingLatency["outputLatency"] = session.outputLatency
    hardware["inspectingLatency"] = inspectingLatency
    
    hardware["outputVolume"] = session.outputVolume
    
    var inputChannels: [String: Any] = [:]
    inputChannels["preferredInputNumberOfChannels"] = session.preferredInputNumberOfChannels
    inputChannels["inputNumberOfChannels"] = session.inputNumberOfChannels
    inputChannels["maximumInputNumberOfChannels"] = session.maximumInputNumberOfChannels
    hardware["inputChannels"] = inputChannels
    
    var outputChannels: [String: Any] = [:]
    outputChannels["preferredOutputNumberOfChannels"] = session.preferredOutputNumberOfChannels
    outputChannels["outputNumberOfChannels"] = session.outputNumberOfChannels
    outputChannels["maximumOutputNumberOfChannels"] = session.maximumOutputNumberOfChannels
    hardware["outputChannels"] = outputChannels
    
    
    var info: [String: Any] = [:]
    info["category"] = category
    info["mode"] = mode
    info["categoryOptions"] = categoryOptions
    info["flags"] = flags
    
    if isDetail {
        info["renderingMode"] = renderingMode
        info["currentRoute"] = currentRoute
        info["hardware"] = hardware
    }
    
    do {
        let jsonData = try JSONSerialization.data(withJSONObject: info, options: [.prettyPrinted, .sortedKeys])
        if let jsonString = String(data: jsonData, encoding: .utf8) {
            let utfText: UnsafePointer<CChar>? = (jsonString as NSString).utf8String
            let pointer: UnsafeMutablePointer<Int8> = UnsafeMutablePointer<Int8>.allocate(capacity: (8 * jsonString.count) + 1)
            return strcpy(pointer, utfText)
        }
    } catch {
        logger.error("[Swift]: Failed to serialize JSON: \(error)")
    }
    
    return nil
}
