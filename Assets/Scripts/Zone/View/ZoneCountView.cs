using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class ZoneCountView : MonoBehaviour
    {
        public ZoneNumberView ZoneNumberViewPrefab;
        public ZoneNumberView[] ZoneNumberViewArray; // Prewarm for performance

        public Transform ZoneNumberContentParent;
    }
}
