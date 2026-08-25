using System;
using System.Collections.Generic;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    [CreateAssetMenu(menuName = "CaseStudy/Sound Library", fileName = "SoundLibrary")]
    public class SoundLibraryAsset : ScriptableObject
    {
        [Header("Wheel")]
        [SerializeField] private AudioClip _wheelSpin;

        [SerializeField] private AudioClip[] _explosions = Array.Empty<AudioClip>();

        [Header("Reward Flight")]
        [SerializeField] private AudioClip _rewardAppear;
        [SerializeField] private AudioClip _rewardImpact;

        [Header("UI")]
        [SerializeField] private AudioClip _button;

        public AudioClip WheelSpin => _wheelSpin;

        public IReadOnlyList<AudioClip> Explosions => _explosions;

        public AudioClip RewardAppear => _rewardAppear;

        public AudioClip RewardImpact => _rewardImpact;

        public AudioClip Button => _button;
    }
}
