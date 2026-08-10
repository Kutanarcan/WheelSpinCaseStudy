using System;

namespace CaseStudy.WheelSpin
{
    public class PopupPresenter
    {
        private readonly CashoutPopup _cashoutPopup;
        private readonly RevivePopup _revivePopup;
        private readonly ItemRegistry _registry;
        private readonly RewardLedger _rewards;

        private bool _isBound;

        public event Action ClaimClicked;
        public event Action ReviveClicked;
        public event Action GiveUpClicked;

        public PopupPresenter(
            CashoutPopup cashoutPopup,
            RevivePopup revivePopup,
            ItemRegistry registry,
            RewardLedger rewards)
        {
            _cashoutPopup = cashoutPopup;
            _revivePopup = revivePopup;
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _rewards = rewards ?? throw new ArgumentNullException(nameof(rewards));
        }

        public void Initialize()
        {
            BindButtons();
            HideAll();
        }

        public void Deinitialize()
        {
            UnbindButtons();
            HideAll();

            if (_cashoutPopup != null)
                _cashoutPopup.Clear();

            ClaimClicked = null;
            ReviveClicked = null;
            GiveUpClicked = null;
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

        private void BindButtons()
        {
            if (_isBound)
                return;

            _isBound = true;

            ActionButtonView claim = GetClaimButton();

            if (claim != null)
                claim.Click += HandleClaimClicked;

            ActionButtonView revive = GetReviveButton();

            if (revive != null)
                revive.Click += HandleReviveClicked;

            ActionButtonView giveUp = GetGiveUpButton();

            if (giveUp != null)
                giveUp.Click += HandleGiveUpClicked;
        }

        private void UnbindButtons()
        {
            if (!_isBound)
                return;

            _isBound = false;

            ActionButtonView claim = GetClaimButton();

            if (claim != null)
                claim.Click -= HandleClaimClicked;

            ActionButtonView revive = GetReviveButton();

            if (revive != null)
                revive.Click -= HandleReviveClicked;

            ActionButtonView giveUp = GetGiveUpButton();

            if (giveUp != null)
                giveUp.Click -= HandleGiveUpClicked;
        }

        private void HandleClaimClicked() => ClaimClicked?.Invoke();

        private void HandleReviveClicked() => ReviveClicked?.Invoke();

        private void HandleGiveUpClicked() => GiveUpClicked?.Invoke();

        private ActionButtonView GetClaimButton()
            => _cashoutPopup != null ? _cashoutPopup.ClaimButton : null;

        private ActionButtonView GetReviveButton()
        {
            if (_revivePopup == null || _revivePopup.ReviveButtonView == null)
                return null;

            return _revivePopup.ReviveButtonView.ActionButtonView;
        }

        private ActionButtonView GetGiveUpButton()
            => _revivePopup != null ? _revivePopup.GiveUpButtonView : null;
    }
}
