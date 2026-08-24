using UnityEngine;
using System.IO;
using System.Collections;

public class AspectRatioScreenshot : MonoBehaviour
{
    public Camera targetCamera;

    public void TakeScreenshot(int width, float aspectW, float aspectH, string fileName)
    {
        int height = Mathf.RoundToInt(width * (aspectH / aspectW));

        RenderTexture rt = new RenderTexture(width, height, 24);
        targetCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);

        targetCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();

        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenShot.EncodeToPNG();
        string path = Path.Combine(Application.persistentDataPath, fileName + ".png");
        File.WriteAllBytes(path, bytes);

        Debug.Log("Kaydedildi: " + path);
    }

    void Start()
    {
        TakeScreenshot(1920, 20f, 9f, "screenshot_20_9");
        TakeScreenshot(1920, 16f, 9f, "screenshot_16_9");
        TakeScreenshot(1920, 4f, 3f, "screenshot_4_3");
    }
}