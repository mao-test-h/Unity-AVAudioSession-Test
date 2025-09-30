#include "Unity/UnityInterface.h"

#ifdef __cplusplus
extern "C" {
#endif

void AVAudioSessionBridge_UnitySetAudioSessionActive(int active)
{
    UnitySetAudioSessionActive(active);
}

#ifdef __cplusplus
}
#endif
