using System;
using System.Collections;
using UnityEngine;

namespace ThePattern.Unity.Sounds
{
    public sealed class SoundChannel : MonoBehaviour
    {
        public DataBind<bool> IsPlayed = new DataBind<bool>();
        public AudioSource _source = (AudioSource)null;
        private bool _isBgmSound;
        private int _clipIndex = 0;
        private AudioClip[] _clips;
        private bool _musicAudioSourcePaused;

        private void Awake()
        {
            this.InitSource();
            this.gameObject.hideFlags = HideFlags.DontSave;
        }

        private void InitSource()
        {
            if (_source)
                return;
            _source = this.gameObject.AddComponent<AudioSource>();
        }

        private void BeforeStart()
        {
            _source.mute = SoundHandler.Instance.IsMuted;
            _source.volume = _isBgmSound ? SoundHandler.Instance.BGMVolume : SoundHandler.Instance.SFXVolume;
        }

        public void SetClip(AudioClip clip, bool isBackgroundMusic)
        {
            AudioClip[] clips;
            if (clip)
                clips = new AudioClip[1] { clip };
            else
                clips = null;
            int num = isBackgroundMusic ? 1 : 0;
            this.SetClip(clips, num != 0);
        }

        public void SetClip(AudioClip[] clips, bool isBackgroundMusic)
        {
            this.InitSource();
            _isBgmSound = isBackgroundMusic;
            if (clips == null)
                this.Stop();
            else
                _clips = clips;
        }

        public AudioClip GetCurrentClip() => this._clips[this._clipIndex];

        public void Play(bool isLoop = false, bool forceRestart = false)
        {
            this.BeforeStart();
            _clipIndex = 0;
            _source.clip = this.GetCurrentClip();
            _source.loop = isLoop;
            if (forceRestart || !this.IsPlaying)
            {
                _source.Play();
                IsPlayed.Value = true;
            }
            if (!_source.loop)
                this.StartCoroutine(IeCheckToEnded());
            if (_clips.Length <= 1)
                return;
            this.StartCoroutine(IeWaitMoveNext());
        }

        private IEnumerator IeWaitMoveNext()
        {
            float time = GetCurrentClip().length;
            float fadeTimeIn = 0.0f;
            float fadeTimeOut = 0.0f;
            fadeTimeIn = (double)time <= 2.0 ? (fadeTimeOut = time / 3f) : (fadeTimeOut = 1f);
            FadeAudio(1f, fadeTimeIn, fadeFromValue: 0.0f);
            FadeAudio(0.0f, fadeTimeOut, time - fadeTimeOut);
            yield return new WaitForSeconds(time - Time.deltaTime * 2f);
            ++_clipIndex;
            if (_clipIndex >= _clips.Length)
                _clipIndex = 0;
            _source.clip = GetCurrentClip();
            _source.Play();
            IsPlayed.Value = true;
            StartCoroutine(IeWaitMoveNext());
        }

        private IEnumerator IeCheckToEnded()
        {
            yield return new WaitWhile(() => this.IsPlaying || this._musicAudioSourcePaused);
            SetEmpty();
        }

        public void Stop()
        {
            StopAllCoroutines();
            _source.Stop();
            SetEmpty();
        }

        public void SetVolume(float value) => _source.volume = value;

        public void SetMute(bool isMuted) => _source.mute = isMuted;

        public bool IsEmpty => !_source.clip;

        public bool IsPlaying => _source && _source.isPlaying;

        public void SetEmpty()
        {
            IsPlayed.Value = false;
            _source.clip = null;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (!_source)
                return;
            if (pauseStatus && _source.isPlaying)
            {
                _source.Pause();
                _musicAudioSourcePaused = true;
            }
            else if (!pauseStatus && _musicAudioSourcePaused)
            {
                _source.UnPause();
                _musicAudioSourcePaused = false;
            }
        }

        public void FadeAudio(float fadeToVolume, float duration = 0.0f, float delay = 0.0f, float fadeFromValue = 1f) => this.StartCoroutine(IeFadeAudio(duration, fadeToVolume, delay, fadeFromValue));

        private IEnumerator IeFadeAudio(
          float duration,
          float fadeToVolume,
          float delay,
          float fadeFromValue = 1f)
        {
            yield return new WaitForSeconds(delay);
            float fadeFromVolume = SoundHandler.Instance.GetVolumeFromValue(_isBgmSound, fadeFromValue);
            fadeToVolume = SoundHandler.Instance.GetVolumeFromValue(_isBgmSound, fadeToVolume);
            float elapsed = 0.0f;
            if (duration <= 0.0f)
                duration = _source.clip.length;
            while (duration > 0.0)
            {
                float t = elapsed / duration;
                _source.volume = Mathf.Lerp(fadeFromVolume, fadeToVolume, t);
                elapsed += Time.deltaTime;
                yield return null;
                if (!_source.loop && Mathf.Abs(duration - elapsed) < 1.40129846432482E-45)
                    break;
            }
            this.SetEmpty();
        }
    }
}
