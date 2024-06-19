using UnityEngine;

namespace ThePattern.Unity.Sounds
{
    public class SoundHandler : Singleton<SoundHandler>
    {
        private ISoundChannelList _channelList;
        private const float DEFAULT_MAX_VOLUME_BGM = 0.8f;
        private const float DEFAULT_MAX_VOLUME_SFX = 1f;
        private float _maxVolumeBGM = 0.8f;
        private float _maxVolumeSFX = 1f;
        private float _currentVolumeBGM = 0.8f;
        private float _currentVolumeSFX = 1f;
        private bool _isMuted = false;

        protected override void Init()
        {
            this.gameObject.isStatic = true;
            this.gameObject.hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        public static ISoundChannelList ChannelHandle
        {
            get
            {
                if (SoundHandler.Instance._channelList == null)
                    SoundHandler.Instance._channelList = new DefaultSoundChannelList();
                return SoundHandler.Instance._channelList;
            }
            set => SoundHandler.Instance._channelList = value;
        }

        public void SetMasterVolume(float maxVolumeBGM, float maxVolumeSFX)
        {
            _maxVolumeBGM = Mathf.Clamp01(maxVolumeBGM);
            SoundHandler.ChannelHandle.ChangeVolume(true, BGMVolume);
            _maxVolumeSFX = Mathf.Clamp01(maxVolumeSFX);
            SoundHandler.ChannelHandle.ChangeVolume(false, SFXVolume);
        }

        public bool IsMuted
        {
            get => _isMuted;
            set
            {
                _isMuted = value;
                SoundHandler.ChannelHandle.SetMute(value);
            }
        }

        public float BGMVolume
        {
            get => /*_isMuted ? 0.0f : _currentVolumeBGM*/ Module.musicFx;
            set
            {
                //_currentVolumeBGM = Mathf.Clamp(0.0f, value * _maxVolumeBGM, _maxVolumeBGM);
                Module.musicFx = value;
                SoundHandler.ChannelHandle.ChangeVolume(true, Module.musicFx);
            }
        }

        public float SFXVolume
        {
            get =>/* _isMuted ? 0.0f : _currentVolumeSFX*/ Module.soundFx;
            set
            {
                Module.soundFx = value;
                //_currentVolumeSFX = Mathf.Clamp(0.0f, value * _maxVolumeSFX, _maxVolumeSFX);
                SoundHandler.ChannelHandle.ChangeVolume(false, Module.soundFx);
            }
        }

        public float GetVolumeFromValue(bool isBGM, float value) => isBGM ? Mathf.Clamp(0.0f, value * BGMVolume, BGMVolume) : Mathf.Clamp(0.0f, value * SFXVolume, SFXVolume);

        public void StopBGM() => SoundHandler.ChannelHandle.GetBGMChannel().Stop();

        public SoundChannel PlayBGM(AudioClip[] clips)
        {
            SoundChannel bgmChannel = SoundHandler.ChannelHandle.GetBGMChannel();
            bgmChannel.SetClip(clips, true);
            bgmChannel.Play(true);
            return bgmChannel;
        }

        public SoundChannel PlayBGM(AudioClip clip)
        {
            SoundChannel bgmChannel = SoundHandler.ChannelHandle.GetBGMChannel();
            bgmChannel.SetClip(clip, true);
            bgmChannel.Play(true);
            return bgmChannel;
        }

        public SoundChannel PlaySFX(AudioClip clip, bool isLoop = false)
        {
            SoundChannel emptyChannel = SoundHandler.ChannelHandle.GetEmptyChannel();
            emptyChannel.SetClip(clip, false);
            emptyChannel.Play(isLoop);
            return emptyChannel;
        }

        public SoundChannel FadeSound(
          AudioClip clip,
          float fadeToVolume,
          bool isLoop = false,
          float duration = 0.0f,
          float delay = 0.0f,
          float fadeFromValue = 1f)
        {
            SoundChannel emptyChannel = SoundHandler.ChannelHandle.GetEmptyChannel();
            emptyChannel.SetClip(clip, false);
            emptyChannel.Play(isLoop);
            emptyChannel.FadeAudio(fadeToVolume, duration, delay, fadeFromValue);
            return emptyChannel;
        }
    }
}
