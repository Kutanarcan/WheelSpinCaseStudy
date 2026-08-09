using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace CaseStudy.WheelSpin
{
    [DisallowMultipleComponent]
    public class WheelController : MonoBehaviour
    {
        private const int MaxLogLines = 8;

        [SerializeField] private WheelConfigAsset _config;
        [SerializeField] private bool _drawDebugGui = true;

        [Header("Determinism")]
        [SerializeField] private bool _useFixedSeed = true;
        [SerializeField] private int _seed = 1337;

        [Header("Input")]
        [SerializeField] private KeyCode _spinKey = KeyCode.Space;
        [SerializeField] private KeyCode _cashOutKey = KeyCode.C;
        [SerializeField] private KeyCode _restartKey = KeyCode.R;

        private WheelSession _session;
        private readonly Queue<string> _log = new Queue<string>(MaxLogLines);
        private int _lastSliceIndex = -1;

        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            Deinitialize();
        }

        public void Initialize()
        {
            Random random = _useFixedSeed ? new Random(_seed) : new Random();

            WheelTierRuleProvider tierRules = _config.CreateTierRuleProvider();
            //var zoneProvider = new HardcodedZoneProvider(tierRules);
            
            var zoneSet = _config.ZoneSet;
            var zoneProvider = new ScriptableObjectZoneProvider(zoneSet.Zones, tierRules, _config.PenaltyWeight);

            var calculator = new RandomWeightedResultCalculator(random, _config.SliceCount);
            var spinner = new WheelSpinner(calculator, random);

            _lastSliceIndex = -1;
            _log.Clear();

            _session = new WheelSession(zoneProvider, spinner);
            Subscribe(_session);
            _session.StartRun();
        }

        public void Deinitialize()
        {
            if (_session == null) return;

            Unsubscribe(_session);
            _session.ClearListeners();
            _session = null;
        }

        public void Restart()
        {
            Deinitialize();
            Initialize();
        }

        private void Update()
        {
            if (Input.GetKeyDown(_restartKey))
            {
                Restart();
                return;
            }

            if (_session == null || !_session.IsRunActive) return;

            if (Input.GetKeyDown(_spinKey))
            {
                Spin();
            }
            else if (Input.GetKeyDown(_cashOutKey))
            {
                CashOut();
            }
        }

        public void Spin()
        {
            if (_session == null) return;

            if (!_session.TrySpin(out _))
                Append("Spin rejected.");
        }

        public void CashOut()
        {
            if (_session == null) return;
            _session.CashOut();
        }

        private void Subscribe(WheelSession session)
        {
            session.ZoneStarted += HandleZoneStarted;
            session.SpinResolved += HandleSpinResolved;
            session.RunFailed += HandleRunFailed;
            session.RunCashedOut += HandleRunCashedOut;
            session.RunCompleted += HandleRunCompleted;
        }

        private void Unsubscribe(WheelSession session)
        {
            session.ZoneStarted -= HandleZoneStarted;
            session.SpinResolved -= HandleSpinResolved;
            session.RunFailed -= HandleRunFailed;
            session.RunCashedOut -= HandleRunCashedOut;
            session.RunCompleted -= HandleRunCompleted;
        }

        private void HandleZoneStarted(Zone zone)
        {
            _lastSliceIndex = -1;
            Append($"Zone {zone.Index} [{zone.Tier}] ready.");
        }

        private void HandleSpinResolved(SpinResult result)
        {
            _lastSliceIndex = result.SliceIndex;

            string outcome = result.IsPenalty
                ? "PENALTY"
                : $"{result.ItemId} x{result.Amount}";

            Append($"Slice {result.SliceIndex} -> {outcome}");
        }

        private void HandleRunFailed(int zone, long lost)
            => Append($"FAILED at zone {zone}. Lost {lost}.");

        private void HandleRunCashedOut(int zone, long banked)
            => Append($"CASHED OUT at zone {zone} with {banked}.");

        private void HandleRunCompleted(int lastZone, long banked)
            => Append($"COMPLETED after zone {lastZone}. Banked {banked}.");

        private void Append(string line)
        {
            if (_log.Count == MaxLogLines) _log.Dequeue();
            _log.Enqueue(line);
            Debug.Log(line, this);
        }

        private void OnGUI()
        {
            if (!_drawDebugGui) return;

            GUILayout.BeginArea(new Rect(12f, 12f, 460f, Screen.height - 24f), GUI.skin.box);

            if (_session == null)
            {
                GUILayout.Label("Session not initialized.");
                if (GUILayout.Button("Initialize")) Initialize();
                GUILayout.EndArea();
                return;
            }

            DrawState();
            GUILayout.Space(8f);
            DrawSlices();
            GUILayout.Space(8f);
            DrawLog();
            GUILayout.Space(8f);
            DrawButtons();

            GUILayout.EndArea();
        }

        private void DrawState()
        {
            Zone zone = _session.CurrentZone;

            GUILayout.Label($"Zone        : {_session.ZoneIndex}");
            GUILayout.Label($"Tier        : {(zone != null ? zone.Tier.ToString() : "-")}");
            GUILayout.Label($"Accumulated : {_session.Accumulated}");
            GUILayout.Label($"Run active  : {_session.IsRunActive}");
        }

        private void DrawSlices()
        {
            Zone zone = _session.CurrentZone;

            if (zone == null)
            {
                GUILayout.Label("No active zone.");
                return;
            }

            Wheel wheel = zone.Wheel;

            int total = 0;
            for (int i = 0; i < wheel.Length; i++)
                total += wheel[i].Weight;

            GUILayout.Label("Slices:");

            for (int i = 0; i < wheel.Length; i++)
            {
                WheelSlice slice = wheel[i];

                string marker = i == _lastSliceIndex ? ">" : " ";
                string content = slice.Type == SliceType.Penalty
                    ? "PENALTY"
                    : $"{slice.ItemId} x{slice.Amount}";
                float percent = total > 0 ? slice.Weight * 100f / total : 0f;

                GUILayout.Label($"{marker} [{i}] {content,-18} w:{slice.Weight,3}  {percent,5:0.0}%");
            }
        }

        private void DrawLog()
        {
            var builder = new StringBuilder();
            foreach (string line in _log) builder.AppendLine(line);

            GUILayout.Label("Log:");
            GUILayout.Label(builder.ToString());
        }

        private void DrawButtons()
        {
            GUILayout.BeginHorizontal();

            GUI.enabled = _session.IsRunActive;
            if (GUILayout.Button($"Spin ({_spinKey})")) Spin();
            if (GUILayout.Button($"Cash Out ({_cashOutKey})")) CashOut();
            GUI.enabled = true;

            if (GUILayout.Button($"Restart ({_restartKey})")) Restart();

            GUILayout.EndHorizontal();
        }
    }
}