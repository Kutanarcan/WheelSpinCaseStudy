namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// Shows and hides the two popups and fills the cashout list. Button wiring lives in
    /// <see cref="WheelInputBinder"/> — this class only exposes which buttons exist.
    /// </summary>
    public class PopupPresenter
    {
        private readonly CashoutPopup _cashoutPopup;
        private readonly RevivePopup _revivePopup;
        private readonly ItemRegistry _registry;
        private readonly RewardLedger _rewards;

        public PopupPresenter(
            CashoutPopup cashoutPopup,
            RevivePopup revivePopup,
            ItemRegistry registry,
            RewardLedger rewards)
        {
            _cashoutPopup = cashoutPopup;
            _revivePopup = revivePopup;
            _registry = registry;
            _rewards = rewards;
        }

        public ActionButtonView ClaimButton
            => _cashoutPopup != null ? _cashoutPopup.ClaimButton : null;

        public ActionButtonView ReviveButton
            => _revivePopup != null && _revivePopup.ReviveButtonView != null
                ? _revivePopup.ReviveButtonView.ActionButtonView
                : null;

        public ActionButtonView GiveUpButton
            => _revivePopup != null ? _revivePopup.GiveUpButtonView : null;

        public void Initialize() => HideAll();

        public void Deinitialize()
        {
            HideAll();

            if (_cashoutPopup != null)
                _cashoutPopup.Clear();
        }

        public void ResetForNewRun() => HideAll();

        public void HideAll()
        {
            if (_cashoutPopup != null)
                _cashoutPopup.Hide();

            if (_revivePopup != null)
                _revivePopup.Hide();
        }

        public void ShowCashout()
        {
            if (_cashoutPopup == null)
                return;

            _cashoutPopup.Bind(_rewards.Entries, _registry);
            _cashoutPopup.Show();
        }

        public void ShowRevive()
        {
            if (_revivePopup != null)
                _revivePopup.Show();
        }
    }
}
