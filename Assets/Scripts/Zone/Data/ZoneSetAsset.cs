using System;
using System.Collections.Generic;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    [CreateAssetMenu(menuName = "CaseStudy/Zone Set", fileName = "ZoneSet")]
    public class ZoneSetAsset : ScriptableObject
    {
        [SerializeField] private ZoneAsset[] _zones = Array.Empty<ZoneAsset>();

        public IReadOnlyList<ZoneAsset> Zones => _zones;
    }
}