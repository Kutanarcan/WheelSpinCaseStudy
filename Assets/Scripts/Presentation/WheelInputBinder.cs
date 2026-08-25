using System;

namespace CaseStudy.WheelSpin
{
    public sealed class WheelInputBinder
    {
        private readonly WheelSceneView _view;
        private readonly AudioManager _audio;

        private PopupPresenter _popups;
        private bool _isBound;

        public event Action SpinClicked;
        public event Action CashOutClicked;
        public event Action ClaimClicked;
        public event Action ReviveClicked;
        public event Action GiveUpClicked;

        public WheelInputBinder(WheelSceneView view)
        {
            _view = view;
            _audio = view != null ? view.AudioManager : null;
        }

        public void Bind(PopupPresenter popups)
        {
            if (_isBound)
                return;

            _isBound = true;
            _popups = popups;

            Subscribe(SpinButton(), HandleSpin);
            Subscribe(CashOutButton(), HandleCashOut);
            Subscribe(ClaimButton(), HandleClaim);
            Subscribe(ReviveButton(), HandleRevive);
            Subscribe(GiveUpButton(), HandleGiveUp);
        }

        public void Unbind()
        {
            if (!_isBound)
                return;

            _isBound = false;

            Unsubscribe(SpinButton(), HandleSpin);
            Unsubscribe(CashOutButton(), HandleCashOut);
            Unsubscribe(ClaimButton(), HandleClaim);
            Unsubscribe(ReviveButton(), HandleRevive);
            Unsubscribe(GiveUpButton(), HandleGiveUp);

            _popups = null;

            SpinClicked = null;
            CashOutClicked = null;
            ClaimClicked = null;
            ReviveClicked = null;
            GiveUpClicked = null;
        }

        private static void Subscribe(ActionButtonView button, Action handler)
        {
            if (button != null)
                button.Click += handler;
        }

        private static void Unsubscribe(ActionButtonView button, Action handler)
        {
            if (button != null)
                button.Click -= handler;
        }

        private void HandleSpin() => Raise(SpinClicked);

        private void HandleCashOut() => Raise(CashOutClicked);

        private void HandleClaim() => Raise(ClaimClicked);

        private void HandleRevive() => Raise(ReviveClicked);

        private void HandleGiveUp() => Raise(GiveUpClicked);

        private void Raise(Action clicked)
        {
            if (_audio != null)
                _audio.PlayButton();

            clicked?.Invoke();
        }

        private ActionButtonView SpinButton()
        {
            if (_view == null || _view.WheelView == null || _view.WheelView.SpinButtonView == null)
                return null;

            return _view.WheelView.SpinButtonView.ActionButton;
        }

        private ActionButtonView CashOutButton()
            => _view != null && _view.RewardHolderView != null
                ? _view.RewardHolderView.CashOutButtonView
                : null;

        private ActionButtonView ClaimButton() => _popups != null ? _popups.ClaimButton : null;

        private ActionButtonView ReviveButton() => _popups != null ? _popups.ReviveButton : null;

        private ActionButtonView GiveUpButton() => _popups != null ? _popups.GiveUpButton : null;
    }
}
