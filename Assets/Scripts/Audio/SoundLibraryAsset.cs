using System;
using System.Collections.Generic;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// The clips the game plays, kept out of the scene so audio can be swapped without touching a
    /// prefab. Holds data only — picking between variants is the player's job.
    /// </summary>
    [CreateAssetMenu(menuName = "CaseStudy/Sound Library", fileName = "SoundLibrary")]
    public class SoundLibraryAsset : ScriptableObject
    {
        [Header("Wheel")]
        [SerializeField] private AudioClip _wheelSpin;

        [Tooltip("One is picked at random per blast, so repeated explosions do not sound identical.")]
        [SerializeField] private AudioClip[] _explosions = Array.Empty<AudioClip>();

        [Header("Reward Flight")]
        [Tooltip("Fires once per icon as it pops out of the slice.")]
        [SerializeField] private AudioClip _rewardAppear;
        [Tooltip("Fires once per icon as it lands on the reward board.")]
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
