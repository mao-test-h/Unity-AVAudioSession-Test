namespace AVAudioSessionBridge
{
    public static class AudioSessionFactory
    {
        private static IAudioSession _instance;

        public static IAudioSession Create()
        {
            if (_instance != null)
            {
                return _instance;
            }

#if UNITY_IOS && !UNITY_EDITOR
            _instance = new AVAudioSessionBridge();
#else
            _instance = new DummyAudioSession();
#endif
            return _instance;
        }
    }
}
