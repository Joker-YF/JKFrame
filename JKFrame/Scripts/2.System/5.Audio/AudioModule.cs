using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JKFrame
{
    public class AudioModule : MonoBehaviour
    {
        private static GameObjectPoolModule poolModule = new GameObjectPoolModule();

        [SerializeField, LabelText("背景音乐播放器")]
        private AudioSource BGAudioSource;
        [SerializeField, LabelText("效果播放器预制体")]
        private GameObject EffectAudioPlayPrefab;
        [SerializeField, LabelText("对象池预设播放器数量")]
        private int EffectAudioDefaultQuantity = 20;

        // 场景中生效的所有特效音乐播放器
        private List<AudioSource> audioPlayList;

        #region 音量、播放控制
        [SerializeField, Range(0, 1), OnValueChanged("UpdateAllAudioPlay")]
        private float globalVolume;
        public float GlobalVolume
        {
            get => globalVolume;
            set
            {
                if (globalVolume == value) return;
                globalVolume = value;
                UpdateAllAudioPlay();
            }
        }
        [SerializeField]
        [Range(0, 1)]
        [OnValueChanged("UpdateBGAudioPlay")]
        private float bgVolume;
        public float BGVolume
        {
            get => bgVolume;
            set
            {
                if (bgVolume == value) return;
                bgVolume = value;
                UpdateBGAudioPlay();
            }
        }

        [SerializeField]
        [Range(0, 1)]
        [OnValueChanged("UpdateEffectAudioPlay")]
        private float effectVolume;
        public float EffectVolume
        {
            get => effectVolume;
            set
            {
                if (effectVolume == value) return;
                effectVolume = value;
                UpdateEffectAudioPlay();
            }
        }

        [SerializeField]
        [OnValueChanged("UpdateMute")]
        private bool isMute = false;
        public bool IsMute
        {
            get => isMute;
            set
            {
                if (isMute == value) return;
                isMute = value;
                UpdateMute();
            }
        }
        [SerializeField]
        [OnValueChanged("UpdateLoop")]
        private bool isLoop = true;
        public bool IsLoop
        {
            get => isLoop;
            set
            {
                if (isLoop == value) return;
                isLoop = value;
                UpdateLoop();
            }
        }
        private bool isPause = false;
        public bool IsPause
        {
            get => isPause;
            set
            {
                if (isPause == value) return;
                isPause = value;
                if (isPause)
                {
                    BGAudioSource.Pause();
                }
                else
                {
                    BGAudioSource.UnPause();
                }
                UpdateEffectAudioPlay();
            }
        }
        /// <summary>
        /// 更新全部播放器类型
        /// </summary>
        private void UpdateAllAudioPlay()
        {
            UpdateBGAudioPlay();
            UpdateEffectAudioPlay();
        }

        /// <summary>
        /// 更新背景音乐
        /// </summary>
        private void UpdateBGAudioPlay()
        {
            BGAudioSource.volume = bgVolume * globalVolume;
        }

        /// <summary>
        /// 更新特效音乐播放器
        /// </summary>
        private void UpdateEffectAudioPlay()
        {
            if (audioPlayList == null) return;
            // 倒序遍历
            for (int i = audioPlayList.Count - 1; i >= 0; i--)
            {
                if (audioPlayList[i] != null)
                {
                    SetEffectAudioPlay(audioPlayList[i]);
                }
                else
                {
                    audioPlayList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 设置特效音乐播放器
        /// </summary>
        private void SetEffectAudioPlay(AudioSource audioPlay, float spatial = -1)
        {
            audioPlay.mute = isMute;
            audioPlay.volume = effectVolume * globalVolume;
            if (spatial != -1)
            {
                audioPlay.spatialBlend = spatial;
            }
            if (isPause)
            {
                audioPlay.Pause();
            }
            else
            {
                audioPlay.UnPause();
            }
        }

        /// <summary>
        /// 更新背景音乐静音情况
        /// </summary>
        private void UpdateMute()
        {
            BGAudioSource.mute = isMute;
            UpdateEffectAudioPlay();
        }

        /// <summary>
        /// 更新背景音乐循环
        /// </summary>
        private void UpdateLoop()
        {
            BGAudioSource.loop = isLoop;

        }
        #endregion

        public void Init()
        {
            Transform poolRoot = new GameObject("AudioPlayerPoolRoot").transform;
            poolRoot.SetParent(transform);
            poolModule.Init(poolRoot);
            poolModule.InitObjectPool(EffectAudioPlayPrefab, -1, EffectAudioDefaultQuantity);
            audioPlayList = new List<AudioSource>(EffectAudioDefaultQuantity);
            UpdateAllAudioPlay();
        }

        #region 背景音乐
        public void PlayBGAudio(AudioClip clip, bool loop = true, float volume = -1)
        {
            BGAudioSource.clip = clip;
            IsLoop = loop;
            if (volume != -1)
            {
                BGVolume = volume;
            }
            BGAudioSource.Play();
        }


        private static Coroutine bgWithClipsCoroutine;
        /// <summary>
        /// 使用音效数组播放背景音乐，自动循环
        /// </summary>
        public void PlayBGAudioWithClips(AudioClip[] clips, float volume = -1)
        {
            bgWithClipsCoroutine = MonoSystem.Start_Coroutine(DoPlayBGAudioWithClips(clips, volume));
        }

        private IEnumerator DoPlayBGAudioWithClips(AudioClip[] clips, float volume = -1)
        {
            if (volume != -1)
            {
                BGVolume = volume;
            }
            int currIndex = 0;
            while (true)
            {
                AudioClip clip = clips[currIndex];
                BGAudioSource.clip = clip;
                BGAudioSource.Play();
                yield return CoroutineTool.WaitForSeconds(clip.length);
                currIndex++;
                if (currIndex >= clips.Length) currIndex = 0;
            }
        }
        public void StopBGAudio()
        {
            if (bgWithClipsCoroutine != null) MonoSystem.Stop_Coroutine(bgWithClipsCoroutine);
            BGAudioSource.Stop();
            BGAudioSource.clip = null;
        }

        public void PauseBGAudio()
        {
            BGAudioSource.Pause();
        }

        public void UnPauseBGAudio()
        {
            BGAudioSource.UnPause();
        }

        #endregion

        #region 特效音乐
        private Transform audioPlayRoot = null;
        /// <summary>
        /// 获取音乐播放器
        /// </summary>
        /// <returns></returns>
        private AudioSource GetAudioPlay(bool is3D = true)
        {
            if (audioPlayRoot == null)
            {
                audioPlayRoot = new GameObject("AudioPlayRoot").transform;
            }
            // 从对象池中获取播放器
            GameObject audioPlay = poolModule.GetObject("AudioPlay", audioPlayRoot);
            if (audioPlay.IsNull())
            {
                audioPlay = GameObject.Instantiate(EffectAudioPlayPrefab, audioPlayRoot);
                audioPlay.name = EffectAudioPlayPrefab.name;
            }
            AudioSource audioSource = audioPlay.GetComponent<AudioSource>();
            SetEffectAudioPlay(audioSource, is3D ? 1f : 0f);
            audioPlayList.Add(audioSource);
            return audioSource;
        }

        /// <summary>
        /// 回收播放器
        /// </summary>
        private void RecycleAudioPlay(AudioSource audioSource, AudioClip clip, bool autoReleaseClip, Action callBak)
        {
            StartCoroutine(DoRecycleAudioPlay(audioSource, clip, autoReleaseClip, callBak));
        }

        private IEnumerator DoRecycleAudioPlay(AudioSource audioSource, AudioClip clip, bool autoReleaseClip, Action callBak)
        {
            // 延迟 Clip的长度（秒）
            yield return CoroutineTool.WaitForSeconds(clip.length);
            // 放回池子
            if (audioSource != null)
            {
                audioPlayList.Remove(audioSource);
                poolModule.PushObject(audioSource.gameObject);
                if (autoReleaseClip) ResSystem.UnloadAsset(clip);
                callBak?.Invoke();
            }
        }

        public void PlayOnShot(AudioClip clip, Component component = null, bool autoReleaseClip = false, float volumeScale = 1, bool is3d = true, Action callBack = null)
        {
            // 初始化音乐播放器
            AudioSource audioSource = GetAudioPlay(is3d);
            if (component == null) audioSource.transform.SetParent(null);
            else
            {
                audioSource.transform.SetParent(component.transform);
                // 宿主销毁时，释放父物体
                component.OnDesotry(OnOwerDestory, audioSource);
            }
            audioSource.transform.localPosition = Vector3.zero;
            // 播放一次音效
            audioSource.PlayOneShot(clip, volumeScale);
            // 播放器回收以及回调函数
            callBack += () => PlayOverRemoveOwnerDesotryAction(component);         // 播放结束时移除宿主销毁Action
            RecycleAudioPlay(audioSource, clip, autoReleaseClip, callBack);
        }

        // 宿主销毁时，提前回收
        private void OnOwerDestory(GameObject go, AudioSource audioSource)
        {
            audioSource.transform.SetParent(null);
        }

        // 播放结束时移除宿主销毁Action
        private void PlayOverRemoveOwnerDesotryAction(Component owner)
        {
            if (owner != null) owner.RemoveOnDesotry<AudioSource>(OnOwerDestory);
        }

        public void PlayOnShot(AudioClip clip, Vector3 position, bool autoReleaseClip = false, float volumeScale = 1, bool is3d = true, Action callBack = null)
        {
            // 初始化音乐播放器
            AudioSource audioSource = GetAudioPlay(is3d);
            audioSource.transform.position = position;

            // 播放一次音效
            audioSource.PlayOneShot(clip, volumeScale);
            // 播放器回收以及回调函数
            RecycleAudioPlay(audioSource, clip, autoReleaseClip, callBack);
        }
        #endregion
    }
}