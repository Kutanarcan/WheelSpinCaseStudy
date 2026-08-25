using UnityEngine;
using Random = System.Random;

namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// The composition root: builds the game once, keeps it alive for the scene's lifetime, and
    /// turns button events into session calls. No service locator, no singleton.
    /// </summary>
    [DisallowMultipleComponent]
    public class WheelController : MonoBehaviour
    {
        [SerializeField] private WheelConfigAsset _config;
        [SerializeField] private WheelSceneView _sceneView;

        [Header("Determinism")]
        [SerializeField] private bool _useFixedSeed;
        [SerializeField] private int _seed = 1337;

        private WheelGame _game;
        private WheelInputBinder _binder;

        private void Awake()
        {
            Initialize();
            StartNewRun();
        }

        private void OnDestroy() => Deinitialize();

        public void Initialize()
        {
            Random random = _useFixedSeed ? new Random(_seed) : new Random();

            if (!WheelGame.TryCreate(_config, _sceneView, random, out _game, out string error))
            {
                Debug.LogError($"[{nameof(WheelController)}] {error}", this);
                return;
            }

            BindInput();
        }

        public void Deinitialize()
        {
            UnbindInput();

            if (_game == null)
                return;

            _game.Dispose();
            _game = null;
        }

        public void StartNewRun()
        {
            if (_game == null)
                return;

            _game.Presenter.ResetForNewRun();
            _game.Session.StartRun();
            _game.Presenter.PlayInitial();
        }

        public void Spin()
        {
            if (!CanAct() || !_game.Session.TrySpin(out _))
                return;

            _game.Presenter.Play();
        }

        public void CashOut()
        {
            if (!CanAct() || _game.Rewards.IsEmpty)
                return;

            _game.Session.CashOut();
            _game.Presenter.Play();
        }

        private void BindInput()
        {
            _binder = new WheelInputBinder(_sceneView);

            _binder.SpinClicked += Spin;
            _binder.CashOutClicked += CashOut;
            _binder.ClaimClicked += HandleClaim;
            _binder.ReviveClicked += HandleRevive;
            _binder.GiveUpClicked += HandleGiveUp;

            _binder.Bind(_game.Presenter.Popups);
        }

        private void UnbindInput()
        {
            if (_binder == null)
                return;

            _binder.Unbind();
            _binder = null;
        }

        private void HandleClaim() => StartNewRun();

        private void HandleGiveUp()
        {
            if (_game == null)
                return;

            _game.Session.GiveUp();
            StartNewRun();
        }

        private void HandleRevive()
        {
            if (_game == null || !_game.Session.TryRevive())
                return;

            _game.Presenter.PlayRevive();
        }

        private bool CanAct()
            => _game != null && _game.Session.IsRunActive && !_game.Presenter.IsBusy;
    }
}
