using System;

namespace CaseStudy.WheelSpin
{
    public class PopupPresenter
    {
        private readonly CashoutPopup _cashoutPopup;
        private readonly RevivePopup _revivePopup;
        private readonly ItemRegistry _registry;
        private readonly RewardLedger _rewards;
        private readonly AudioManager _audio;

        private bool _isBound;

        public event Action ClaimClicked;
        public event Action ReviveClicked;
        public event Action GiveUpClicked;

        public PopupPresenter(
            CashoutPopup cashoutPopup,
            RevivePopup revivePopup,
            ItemRegistry registry,
            RewardLedger rewards,
            AudioManager audio)
        {
            _cashoutPopup = cashoutPopup;
            _revivePopup = revivePopup;
            _registry = registry;
            _rewards = rewards;
            _audio = audio;
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

        private void HandleClaimClicked()
        {
            PlayButtonSound();
            ClaimClicked?.Invoke();
        }

        private void HandleReviveClicked()
        {
            PlayButtonSound();
            ReviveClicked?.Invoke();
        }

        private void HandleGiveUpClicked()
        {
            PlayButtonSound();
            GiveUpClicked?.Invoke();
        }

        private void PlayButtonSound()
        {
            if (_audio != null)
                _audio.PlayButton();
        }

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
