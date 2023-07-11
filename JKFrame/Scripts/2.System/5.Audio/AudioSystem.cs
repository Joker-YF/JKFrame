using System;
using UnityEngine;

namespace JKFrame
{
    public static class AudioSystem
    {
        private static AudioModule audioModule;
        public static void Init()
        {
            audioModule = JKFrameRoot.RootTransform.GetComponentInChildren<AudioModule>();
            audioModule.Init();
        }
        public static float GlobalVolume { get => audioModule.GlobalVolume; set { audioModule.GlobalVolume = value; } }
        public static float BGVolume { get => audioModule.BGVolume; set { audioModule.BGVolume = value; } }
        public static float EffectVolume { get => audioModule.EffectVolume; set { audioModule.EffectVolume = value; } }
        public static bool IsMute { get => audioModule.IsMute; set { audioModule.IsMute = value; } }
        public static bool IsLoop { get => audioModule.IsLoop; set { audioModule.IsLoop = value; } }
        public static bool IsPause { get => audioModule.IsPause; set { audioModule.IsPause = value; } }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="clip">音乐片段</param>
        /// <param name="loop">是否循环</param>
        /// <param name="volume">音量，-1代表不设置，采用当前音量</param>
        /// <param name="fadeOutTime">渐出音量花费的时间</param>
        /// <param name="fadeInTime">渐入音量花费的时间</param>
        public static void PlayBGAudio(AudioClip clip, bool loop = true, float volume = -1, float fadeOutTime = 0, float fadeInTime = 0)
            => audioModule.PlayBGAudio(clip, loop, volume, fadeOutTime, fadeInTime);

        /// <summary>
        /// 使用音效数组播放背景音乐，自动循环
        /// </summary>
        /// <param name="fadeOutTime">渐出音量花费的时间</param>
        /// <param name="fadeInTime">渐入音量花费的时间</param>
        public static void PlayBGAudioWithClips(AudioClip[] clips, float volume = -1, float fadeOutTime = 0, float fadeInTime = 0)
            => audioModule.PlayBGAudioWithClips(clips, volume, fadeOutTime, fadeInTime);

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public static void StopBGAudio() => audioModule.StopBGAudio();

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public static void PauseBGAudio() => audioModule.PauseBGAudio();

        /// <summary>
        /// 继续播放背景音乐
        /// </summary>
        public static void UnPauseBGAudio() => audioModule.UnPauseBGAudio();

        /// <summary>
        /// 播放一次特效音乐,并且绑定在某个游戏物体身上
        /// 但是不用担心，游戏物体销毁时，会瞬间解除绑定，回收音效播放器
        /// </summary>
        /// <param name="clip">音效片段</param>
        /// <param name="autoReleaseClip">播放完毕时候自动回收audioClip</param>
        /// <param name="component">挂载组件</param>
        /// <param name="volumeScale">音量 0-1</param>
        /// <param name="is3d">是否3D</param>
        /// <param name="callBack">回调函数-在音乐播放完成后执行</param>
        public static void PlayOneShot(AudioClip clip, Component component = null, bool autoReleaseClip = false, float volumeScale = 1, bool is3d = true, Action callBack = null)
            => audioModule.PlayOneShot(clip, component, autoReleaseClip, volumeScale, is3d, callBack);

        /// <summary>
        /// 播放一次特效音乐
        /// </summary>
        /// <param name="clip">音效片段</param>
        /// <param name="position">播放的位置</param>
        /// <param name="autoReleaseClip">播放完毕时候自动回收audioClip</param>
        /// <param name="volumeScale">音量 0-1</param>
        /// <param name="is3d">是否3D</param>
        /// <param name="callBack">回调函数-在音乐播放完成后执行</param>
        public static void PlayOneShot(AudioClip clip, Vector3 position, bool autoReleaseClip = false, float volumeScale = 1, bool is3d = true, Action callBack = null)
            => audioModule.PlayOneShot(clip, position, autoReleaseClip, volumeScale, is3d, callBack);
    }
}
