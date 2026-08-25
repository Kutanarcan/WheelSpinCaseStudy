using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace CaseStudy.WheelSpin
{
    public class AspectRatioScreenshot : MonoBehaviour
    {
        [FormerlySerializedAs("targetCamera")]
        [SerializeField] private Camera _targetCamera;

        [SerializeField, Min(1)] private int _width = 1920;

        [ContextMenu("Capture All Aspects")]
        public void CaptureAll()
        {
            Capture(20f, 9f, "screenshot_20_9");
            Capture(16f, 9f, "screenshot_16_9");
            Capture(4f, 3f, "screenshot_4_3");
        }

        public void Capture(float aspectWidth, float aspectHeight, string fileName)
        {
            if (_targetCamera == null || aspectWidth <= 0f || aspectHeight <= 0f)
                return;

            int height = Mathf.RoundToInt(_width * (aspectHeight / aspectWidth));

            var renderTexture = new RenderTexture(_width, height, 24);
            var shot = new Texture2D(_width, height, TextureFormat.RGB24, false);

            _targetCamera.targetTexture = renderTexture;
            _targetCamera.Render();

            RenderTexture.active = renderTexture;
            shot.ReadPixels(new Rect(0f, 0f, _width, height), 0, 0);
            shot.Apply();

            _targetCamera.targetTexture = null;
            RenderTexture.active = null;

            string path = Path.Combine(Application.persistentDataPath, fileName + ".png");
            File.WriteAllBytes(path, shot.EncodeToPNG());

            DestroyTemporary(renderTexture);
            DestroyTemporary(shot);

            Debug.Log($"[{nameof(AspectRatioScreenshot)}] Saved {path}", this);
        }

        private static void DestroyTemporary(Object target)
        {
            if (Application.isPlaying)
                Destroy(target);
            else
                DestroyImmediate(target);
        }
    }
}
