using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CaseStudy.WheelSpin
{
    public abstract class PopupView : MonoBehaviour
    {
        public Transform PanelRoot;

        [Header("Dim")]
        [SerializeField] private Graphic _dim;
        [SerializeField, Min(0f)] private float _dimDuration = 0.1f;
        [SerializeField, Range(0f, 1f)] private float _dimAlpha = 1f;

        private Sequence _sequence;
        private bool _restCaptured;

        public void Show()
        {
            EnsureRestCaptured();
            KillSequence();

            if (PanelRoot != null)
                PanelRoot.gameObject.SetActive(true);

            MoveToStart();
            SetDimAlpha(0f);

            _sequence = DOTween.Sequence();

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

        protected virtual void CaptureRest() { }

        protected virtual void MoveToStart() { }

        protected virtual void MoveToRest() { }

        protected virtual void AppendContent(Sequence sequence) { }

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
