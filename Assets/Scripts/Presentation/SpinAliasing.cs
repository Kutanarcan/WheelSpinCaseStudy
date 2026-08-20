using DG.Tweening;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    public static class SpinAliasing
    {
        public const float DefaultSafetyFactor = 0.9f;

        public static float MaxSafeDegreesPerSecond(int sliceCount, int fps, float safety = DefaultSafetyFactor)
            => (360f / Mathf.Max(1, sliceCount)) * 0.5f * Mathf.Max(1, fps) * safety;

        public static float PeakVelocityFactor(Ease ease)
        {
            switch (ease)
            {
                case Ease.Linear: return 1f;
                case Ease.OutSine: return Mathf.PI / 2f;
                case Ease.OutQuad: return 2f;
                case Ease.OutCubic: return 3f;
                case Ease.OutQuart: return 4f;
                case Ease.OutQuint: return 5f;
                case Ease.OutExpo: return 6.931f;
                case Ease.OutCirc: return float.PositiveInfinity;
                default: return 4f;
            }
        }

        public static bool Overshoots(Ease ease)
        {
            switch (ease)
            {
                case Ease.InBack:
                case Ease.OutBack:
                case Ease.InOutBack:
                case Ease.InElastic:
                case Ease.OutElastic:
                case Ease.InOutElastic:
                    return true;
                default:
                    return false;
            }
        }

        public static float SafeDuration(float duration, float totalDegrees, Ease ease, int sliceCount, int fps)
        {
            float maxSpeed = MaxSafeDegreesPerSecond(sliceCount, fps);

            if (maxSpeed <= 0f || totalDegrees <= 0f)
                return duration;

            float required = PeakVelocityFactor(ease) * totalDegrees / maxSpeed;

            return float.IsInfinity(required) ? duration : Mathf.Max(duration, required);
        }

        public static float PeakDegreesPerSecond(float totalDegrees, float duration, Ease ease)
            => PeakVelocityFactor(ease) * totalDegrees / Mathf.Max(0.01f, duration);
        public static int CurrentFrameRate()
            => Application.targetFrameRate > 0 ? Application.targetFrameRate : 60;
    }
}
