using Lib.Util;
using UnityEngine;
using UnityEngine.Audio;

namespace Lib.Sound
{
    /// <summary>
    /// サウンド再生管理
    /// </summary>
    public partial class SoundManager : SingletonBehaviour<SoundManager>
    {
        [SerializeField] private AudioMixerGroup    _mixerGroupMaster;
        [SerializeField] private SoundPlayer        _soundPlayerSE;
        [SerializeField] private SoundPlayer        _soundPlayerBGM;
        [SerializeField] private SoundPlayer        _soundPlayerJingle;

        /// <summary>
        /// 初期化
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _soundPlayerSE.Init();
            _soundPlayerBGM.Init();
            _soundPlayerJingle.Init();
        }

        /// <summary>
        /// SEを再生
        /// </summary>
        public void PlaySE(string key, float pitch = 1.0f)
        {
            _soundPlayerSE.Play(key, 0.0f, pitch);
        }

        /// <summary>
        /// BGMを再生
        /// </summary>
        public void PlayBGM(string key)
        {
            _soundPlayerBGM.PlayCrossFade(key);
        }

        /// <summary>
        /// Jingleを再生
        /// </summary>
        public void PlayJingle(string key)
        {
            _soundPlayerJingle.Play(key);
        }

        /// <summary>
        /// SEを停止
        /// </summary>
        public void StopSE(float fadeSec = SoundPlayer.FADE_SEC)
        {
            _soundPlayerSE.Stop(fadeSec);
        }

        /// <summary>
        /// BGMを停止
        /// </summary>
        public void StopBGM(float fadeSec = SoundPlayer.FADE_SEC)
        {
            _soundPlayerBGM.Stop(fadeSec);
        }

        /// <summary>
        /// Jingleを停止
        /// </summary>
        public void StopJingle(float fadeSec = SoundPlayer.FADE_SEC)
        {
            _soundPlayerJingle.Stop(fadeSec);
        }

        /// <summary>
        /// すべて停止
        /// </summary>
        public void StopAll(float fadeSec = SoundPlayer.FADE_SEC)
        {
            _soundPlayerSE.Stop(fadeSec);
            _soundPlayerBGM.Stop(fadeSec);
            _soundPlayerJingle.Stop(fadeSec);
        }

        /// <summary>
        /// マスターのボリュームを設定
        /// </summary>
        public void SetVolumeMaster(float volume)
        {
            _soundPlayerBGM.SetVolume(volume);
            _soundPlayerSE.SetVolume(volume);
        }

        /// <summary>
        /// マスターのボリュームを取得
        /// </summary>
        public float GetVolumeMaster()
        {
            return _soundPlayerBGM.GetVolume();
        }
    }
}
