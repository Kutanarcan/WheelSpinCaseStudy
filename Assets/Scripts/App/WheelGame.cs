using Random = System.Random;

namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// The wired-up game: the rules session, the presentation layer, and the ledger they share.
    /// Built in one pass so the composition root is left with lifecycle and input only.
    /// </summary>
    public sealed class WheelGame
    {
        public readonly WheelSession Session;
        public readonly WheelPresenter Presenter;
        public readonly RewardLedger Rewards;

        private WheelGame(WheelSession session, WheelPresenter presenter, RewardLedger rewards)
        {
            Session = session;
            Presenter = presenter;
            Rewards = rewards;
        }

        /// <summary>
        /// Validates the zone set before anything is built, so a misconfigured project fails with
        /// one readable message instead of a null reference on the first spin.
        /// </summary>
        public static bool TryCreate(
            WheelConfigAsset config,
            WheelSceneView view,
            Random random,
            out WheelGame game,
            out string error)
        {
            game = null;

            WheelTierRuleProvider tierRules = config.CreateTierRuleProvider();
            ItemRegistry registry = config.ItemDatabase.CreateRegistry();

            var zoneProvider = new ScriptableObjectZoneProvider(
                config.ZoneSet.Zones, tierRules, config.PenaltyChance);

            if (!zoneProvider.TryValidate(out error))
                return false;

            var rewards = new RewardLedger();

            var presenter = new WheelPresenter(
                view, registry, tierRules, config.WheelTierViewDatabase, rewards);

            presenter.Initialize(zoneProvider.ZoneCount);

            var spinner = new WheelSpinner(new RandomWeightedResultCalculator(random), random);
            var session = new WheelSession(zoneProvider, spinner, rewards);

            presenter.Subscribe(session);

            game = new WheelGame(session, presenter, rewards);
            return true;
        }

        public void Dispose()
        {
            Presenter.Unsubscribe(Session);
            Presenter.Deinitialize();
            Session.ClearListeners();
        }
    }
}
