namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// The presenter graph and its lifecycle. Holds the wiring that used to sit in
    /// <see cref="WheelPresenter"/>'s constructor, so that class is left with the one job of
    /// deciding what plays when.
    /// </summary>
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
                view.WheelView, registry, view.PenaltySprite, view.PenaltyViewSettings,
                view.SpinSettings,
                wheelTierViewDatabase,
                view.AudioManager);

            Zone = new ZonePresenter(
                view.ZoneCountView, view.ZoneSelectorView, wheelTierViewDatabase, view.CurrentZoneColor,
                view.PastZoneAlpha / 255f, view.SpinSettings, tierRules);

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
