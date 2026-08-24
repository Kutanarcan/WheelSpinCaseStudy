using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// Shared opening behaviour for popups: the dim fades in while the popup's own content animates
    /// on top of it. Hiding is always instant and puts every animated part back at rest, so a popup
    /// dismissed mid-animation cannot leave anything displaced for the next time it opens.
    /// </summary>
    public abstract class PopupView : MonoBehaviour
    {
        public Transform PanelRoot;

        [Header("Dim")]
        [SerializeField] private Graphic _dim;
        [SerializeField, Min(0f)] private float _dimDuration = 0.1f;
        [SerializeField, Range(0f, 1f)] private float _dimAlpha = 1f;

        private Sequence _sequence;
        private bool _restCaptured;

        public bool IsOpening => _sequence != null;

        public void Show()
        {
            EnsureRestCaptured();
            KillSequence();

            if (PanelRoot != null)
                PanelRoot.gameObject.SetActive(true);

            MoveToStart();
            SetDimAlpha(0f);

            _sequence = DOTween.Sequence();

            // Content is added first so it starts at zero; the dim is inserted at zero afterwards
            // and therefore runs alongside it rather than delaying it.
            AppendContent(_sequence);
            InsertDim(_sequence);

            _sequence
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .OnComplete(HandleComplete);
        }

        public void Hide()
        {
            EnsureRestCaptured();
            KillSequence();

            MoveToRest();
            SetDimAlpha(_dimAlpha);

            if (PanelRoot != null)
                PanelRoot.gameObject.SetActive(false);
        }

        /// <summary>Records the authored transform values that the content animates back to.</summary>
        protected virtual void CaptureRest() { }

        /// <summary>Puts the content where the opening animation begins.</summary>
        protected virtual void MoveToStart() { }

        /// <summary>Puts the content back at its authored pose.</summary>
        protected virtual void MoveToRest() { }

        /// <summary>Adds the popup's own tweens; the sequence already starts at zero.</summary>
        protected virtual void AppendContent(Sequence sequence) { }

        /// Captured lazily rather than in Awake: the presenter hides every popup during its own
        /// Awake, and Unity gives no ordering guarantee between the two.
        private void EnsureRestCaptured()
        {
            if (_restCaptured)
                return;

            _restCaptured = true;
            CaptureRest();
        }

        private void InsertDim(Sequence sequence)
        {
            if (_dim == null || _dimDuration <= 0f)
            {
                SetDimAlpha(_dimAlpha);
                return;
            }

            sequence.Insert(0f, _dim.DOFade(_dimAlpha, _dimDuration));
        }

        private void SetDimAlpha(float alpha)
        {
            if (_dim == null)
                return;

            Color color = _dim.color;
            color.a = alpha;
            _dim.color = color;
        }

        private void HandleComplete() => _sequence = null;

        private void KillSequence()
        {
            _sequence?.Kill();
            _sequence = null;
        }
    }
}
