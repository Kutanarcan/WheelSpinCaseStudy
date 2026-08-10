using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class RevivePopup : MonoBehaviour
    {
        public ReviveButtonView ReviveButtonView;
        public ActionButtonView GiveUpButtonView;
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
