using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class RewardHolderView : MonoBehaviour
    {
        public RewardView RewardViewPrefab;
        public RewardView[] RewardViewArray; // Prewarm for performance

        public ActionButtonView CashOutButtonView;

        public Transform RewardContentParent;

    }
}
