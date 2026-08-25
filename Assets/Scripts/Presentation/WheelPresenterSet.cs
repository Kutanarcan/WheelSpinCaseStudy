namespace CaseStudy.WheelSpin
{
    public sealed class WheelPresenterSet
    {
        public readonly WheelSlicePresenter Slice;
        public readonly ZonePresenter Zone;
        public readonly RewardPresenter Reward;
        public readonly RewardFlightPresenter Flight;
        public readonly PenaltyPresenter Penalty;
        public readonly PopupPresenter Popup;

        public WheelPresenterSet(
            WheelSceneView view,
            ItemRegistry registry,
            WheelTierRuleProvider tierRules,
            WheelTierViewDatabase wheelTierViewDatabase,
            RewardLedger rewards)
        {
            Slice = new WheelSlicePresenter(
                view.WheelView, registry, view.PenaltySettings.Sprite, view.PenaltySettings.SliceView,
                view.SpinSettings,
                wheelTierViewDatabase,
                view.AudioManager);

            Zone = new ZonePresenter(
                view.ZoneCountView, view.ZoneSelectorView, wheelTierViewDatabase,
                view.SpinSettings, tierRules);

            Reward = new RewardPresenter(view.RewardHolderView, registry);

            Flight = new RewardFlightPresenter(
                view.RewardFlightView, Reward, registry, view.AudioManager, view.FlightSettings);

            Penalty = new PenaltyPresenter(
                view.WheelView, view.BombExplosionView, view.ShakeRoot, view.AudioManager,
                view.PenaltySettings);

            Popup = new PopupPresenter(view.CashoutPopup, view.RevivePopup, registry, rewards);
        }

        public void Initialize(int zoneCount)
        {
            Zone.Initialize(zoneCount);
            Reward.Initialize();
            Flight.Initialize();
            Penalty.Initialize();
            Popup.Initialize();
        }

        public void Deinitialize()
        {
            Slice.Deinitialize();
            Zone.Deinitialize();
            Reward.Deinitialize();
            Flight.Deinitialize();
            Penalty.Deinitialize();
            Popup.Deinitialize();
        }

        public void ResetForNewRun()
        {
            Slice.ResetForNewRun();
            Zone.ResetForNewRun();
            Reward.ResetForNewRun();
            Flight.ResetForNewRun();
            Penalty.ResetForNewRun();
            Popup.ResetForNewRun();
        }
    }
}
