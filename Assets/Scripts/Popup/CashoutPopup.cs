using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class CashoutPopup : MonoBehaviour
    {
        public RewardView RewardViewPrefab;
        public ActionButtonView ClaimButton;
        public Transform Content;

        public Transform PanelRoot;

        public void Show()
        {
            PanelRoot.gameObject.SetActive(true);
        }

        public void Hide()
        {
            PanelRoot.gameObject.SetActive(false);
        }
    }
}
