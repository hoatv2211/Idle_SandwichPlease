
using System.Collections.Generic;

namespace ThePattern.Unity.Sounds
{
    public interface ISoundChannelList
    {
        SoundChannel GetEmptyChannel();

        SoundChannel GetBGMChannel();

        List<SoundChannel> GetAllChannel();

        List<SoundChannel> GetSFXChannel();

        void SetMute(bool isMuted);

        void ChangeVolume(bool isBGM, float value);
    }
}