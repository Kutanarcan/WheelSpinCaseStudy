using DG.Tweening;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public class RevivePopup : PopupView
    {
        public ReviveButtonView ReviveButtonView;
        public ActionButtonView GiveUpButtonView;

        [Header("Dead Card")]
        [SerializeField] private RectTransform _deadCardRoot;
        [Tooltip("How far below its resting place the card starts.")]
        [SerializeField, Min(0f)] private float _cardRiseDistance = 700f;
        [Tooltip("Z rotation the card starts tilted at, straightening as it rises.")]
        [SerializeField] private float _cardStartRotation = 5f;
        [SerializeField, Min(0f)] private float _cardDuration = 0.35f;
        [SerializeField] private Ease _cardEase = Ease.OutBack;

        [Header("Title")]
        [SerializeField] private RectTransform _titleRoot;
        [Tooltip("How far above its resting place the title starts.")]
        [SerializeField, Min(0f)] private float _titleDropDistance = 350f;
        [SerializeField, Min(0f)] private float _titleDuration = 0.3f;
        [SerializeField] private Ease _titleEase = Ease.OutBack;

        private Vector2 _cardRest;
        private Vector2 _titleRest;

        protected override void CaptureRest()
        {
            if (_deadCardRoot != null)
                _cardRest = _deadCardRoot.anchoredPosition;

            if (_titleRoot != null)
                _titleRest = _titleRoot.anchoredPosition;
        }

        protected override void MoveToStart()
        {
            if (_deadCardRoot != null)
            {
                _deadCardRoot.anchoredPosition = _cardRest + Vector2.down * _cardRiseDistance;
                _deadCardRoot.localEulerAngles = new Vector3(0f, 0f, _cardStartRotation);
            }

            if (_titleRoot != null)
                _titleRoot.anchoredPosition = _titleRest + Vector2.up * _titleDropDistance;
        }

        protected override void MoveToRest()
        {
            if (_deadCardRoot != null)
            {
                _deadCardRoot.anchoredPosition = _cardRest;
                _deadCardRoot.localEulerAngles = Vector3.zero;
            }

            if (_titleRoot != null)
                _titleRoot.anchoredPosition = _titleRest;
        }

        /// The title is appended rather than joined, so it only starts once the card has landed.
        protected override void AppendContent(Sequence sequence)
        {
            if (_deadCardRoot != null && _cardDuration > 0f)
            {
                sequence.Append(_deadCardRoot.DOAnchorPos(_cardRest, _cardDuration)
                    .SetEase(_cardEase));

                sequence.Join(_deadCardRoot.DOLocalRotate(Vector3.zero, _cardDuration)
                    .SetEase(_cardEase));
            }

            if (_titleRoot != null && _titleDuration > 0f)
            {
                sequence.Append(_titleRoot.DOAnchorPos(_titleRest, _titleDuration)
                    .SetEase(_titleEase));
            }
        }
    }
}
