using System.Collections.Generic;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _source;
        [SerializeField] private SoundLibraryAsset _library;
        [SerializeField, Range(0f, 1f)] private float _volume = 1f;

        public bool IsReady => _source != null && _library != null;

        private void OnValidate()
        {
            if (_source == null)
                _source = GetComponent<AudioSource>();
        }

        private void Awake()
        {
            if (_source == null)
                _source = GetComponent<AudioSource>();
        }

        public void PlayWheelSpin() => Play(_library != null ? _library.WheelSpin : null);

        public void PlayExplosion() => Play(PickRandom(_library != null ? _library.Explosions : null));

        public void PlayRewardAppear() => Play(_library != null ? _library.RewardAppear : null);

        public void PlayRewardImpact() => Play(_library != null ? _library.RewardImpact : null);

        public void PlayButton() => Play(_library != null ? _library.Button : null);

        public void Play(AudioClip clip)
        {
            if (clip == null || _source == null)
                return;

            _source.PlayOneShot(clip, _volume);
        }

        private static AudioClip PickRandom(IReadOnlyList<AudioClip> clips)
        {
            if (clips == null || clips.Count == 0)
                return null;

            return clips[Random.Range(0, clips.Count)];
        }
    }
}
