using UnityEngine;
using Random = System.Random;

namespace CaseStudy.WheelSpin
{
    [DisallowMultipleComponent]
    public class WheelController : MonoBehaviour
    {
        [SerializeField] private WheelConfigAsset _config;
        [SerializeField] private WheelSceneView _sceneView;

        [Header("Determinism")]
        [SerializeField] private bool _useFixedSeed;
        [SerializeField] private int _seed = 1337;

        private WheelSession _session;
        private WheelPresenter _presenter;

        public bool IsInitialized => _session != null;

        private void Awake()
        {
            Initialize();
            StartNewRun();
        }

        private void OnDestroy() => Deinitialize();

        public void Initialize()
        {
            Random random = _useFixedSeed ? new Random(_seed) : new Random();

            WheelTierRuleProvider tierRules = _config.CreateTierRuleProvider();
            ItemRegistry registry = _config.ItemDatabase.CreateRegistry();

            var zoneProvider = new ScriptableObjectZoneProvider(
                _config.ZoneSet.Zones, tierRules, _config.PenaltyWeight);

            if (!zoneProvider.TryValidate(out string error))
            {
                Debug.LogError($"[{nameof(WheelController)}] {error}", this);
                return;
            }

            var calculator = new RandomWeightedResultCalculator(random, _config.SliceCount);
            var spinner = new WheelSpinner(calculator, random);

            _presenter = new WheelPresenter(_sceneView, registry, tierRules, _config.SliceCount);
            _presenter.BusyChanged += HandleBusyChanged;
            _presenter.Initialize(zoneProvider.ZoneCount);

            _session = new WheelSession(zoneProvider, spinner);
            _presenter.Subscribe(_session);

            BindButtons();
        }

        public void Deinitialize()
        {
            UnbindButtons();

            if (_presenter != null)
            {
                if (_session != null) _presenter.Unsubscribe(_session);
                _presenter.BusyChanged -= HandleBusyChanged;
                _presenter.Deinitialize();
                _presenter = null;
            }

            if (_session == null)
                return;

            _session.ClearListeners();
            _session = null;
        }
        public void StartNewRun()
        {
            if (_session == null)
                return;

            _presenter.ResetForNewRun();
            _session.StartRun();
            _presenter.PlayInitial();
        }
        public void Reload()
        {
            Deinitialize();
            Initialize();
            StartNewRun();
        }

        public void Spin()
        {
            if (!CanAct()) 
                return;

            if (!_session.TrySpin(out _)) 
                return;

            _presenter.Play();
        }

        public void CashOut()
        {
            if (!CanAct()) return;

            _session.CashOut();
            _presenter.Play();
        }

        private bool CanAct()
        {
            return _session != null
                    && _session.IsRunActive
                    && _presenter != null
                    && !_presenter.IsBusy;
        }

        private void HandleBusyChanged(bool busy)
        {
            SetButtonsInteractable(!busy);
        }

        private void SetButtonsInteractable(bool interactable)
        {
            ActionButtonView spin = GetSpinButton();

            if (spin != null) 
                spin.Interactable = interactable;

            ActionButtonView cashOut = GetCashOutButton();

            if (cashOut != null) 
                cashOut.Interactable = interactable;
        }

        private void BindButtons()
        {
            ActionButtonView spin = GetSpinButton();

            if (spin != null) 
                spin.Click += Spin;

            ActionButtonView cashOut = GetCashOutButton();

            if (cashOut != null) 
                cashOut.Click += CashOut;
        }

        private void UnbindButtons()
        {
            ActionButtonView spin = GetSpinButton();

            if (spin != null)
                spin.Click -= Spin;

            ActionButtonView cashOut = GetCashOutButton();

            if (cashOut != null)
                cashOut.Click -= CashOut;
        }

        private ActionButtonView GetSpinButton()
        {
            if (_sceneView == null || _sceneView.WheelView == null) 
                return null;

            SpinButtonView spinButton = _sceneView.WheelView.SpinButtonView;

            return spinButton != null ? spinButton.ActionButton : null;
        }

        private ActionButtonView GetCashOutButton()
        {
            if (_sceneView == null || _sceneView.RewardHolderView == null) 
                return null;

            return _sceneView.RewardHolderView.CashOutButtonView;
        }
    }
}