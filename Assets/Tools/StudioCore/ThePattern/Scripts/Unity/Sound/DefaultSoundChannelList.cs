
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThePattern.Unity.Sounds
{
    public class DefaultSoundChannelList : ISoundChannelList
    {
        private List<SoundChannel> _listChannel = new List<SoundChannel>();
        private SoundChannel _channelBgm;

        public SoundChannel GetBGMChannel()
        {
            if (!_channelBgm)
                _channelBgm = this.GetEmptyChannel();
            return _channelBgm;
        }

        public List<SoundChannel> GetAllChannel() => _listChannel;
        public List<SoundChannel> GetSFXChannel() => _listChannel.FindAll(p => p != _channelBgm);

        public SoundChannel GetEmptyChannel()
        {
            SoundChannel emptyChannel = _listChannel.Find(c => c.IsEmpty);
            if (!emptyChannel)
            {
                GameObject gameObject = new GameObject(string.Format("Channel_{0:0#}", _listChannel.Count + 1));
                gameObject.transform.SetParent(SoundHandler.Instance.transform);
                emptyChannel = gameObject.AddComponent<SoundChannel>();
                _listChannel.Add(emptyChannel);
            }
            return emptyChannel;
        }

        public void SetMute(bool isMuted) => this.GetAllChannel().ForEach(c => c.SetMute(isMuted));

        public void ChangeVolume(bool isBGM, float value)
        {
            if (isBGM)
                GetBGMChannel().SetVolume(value);
            else
                GetSFXChannel().ForEach(c => c.SetVolume(value));
        }
    }
}
