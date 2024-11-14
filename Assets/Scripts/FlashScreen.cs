using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashScreen : MonoBehaviour
{
    [SerializeField] Image imagePlaceholder;

    private Light directionalLight;
    private float timer;


    private void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= 3f)
        {
            timer = 0f;
        }

        if (timer < 1.5f)
        {
            RenderSettings.ambientLight = Color.white;
            RenderSettings.ambientIntensity = 1f;
            directionalLight.intensity = 1f;
            //StartCoroutine(TakeScreenshotAndShow());

            //stalkerMaterial.color = new Color(stalkerMaterial.color.r, stalkerMaterial.color.g, stalkerMaterial.color.b, Mathf.Lerp(stalkerMaterial.color.a, 0f, Time.deltaTime));
        }
        else
        {
       /*      RenderSettings.ambientLight = Color.black;
            RenderSettings.ambientIntensity = 0f;
            directionalLight.intensity = 0f;
            stalker.SetActive(true);
            //stalkerMaterial.color = new Color(stalkerMaterial.color.r, stalkerMaterial.color.g, stalkerMaterial.color.b, Mathf.Lerp(stalkerMaterial.color.a, 1f, Time.deltaTime));
            StartCoroutine(DeleteScreenshot()); */
        }   
        if (Input.GetKeyDown(KeyCode.H))
        {
        StartCoroutine(TakeScreenshotAndShow());
        }
    }


    private IEnumerator TakeScreenshotAndShow()
    {
        yield return new WaitForEndOfFrame();

        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();

        Texture2D newScreenshot = new Texture2D(screenshot.width, screenshot.height, TextureFormat.RGB24, false);
        newScreenshot.SetPixels(screenshot.GetPixels());
        newScreenshot.Apply();

        Destroy(screenshot);

        Sprite screenshotSprite = Sprite.Create(newScreenshot, new Rect(0, 0, newScreenshot.width, newScreenshot.height)
            , new Vector2(0.5f, 0.5f));

        imagePlaceholder.enabled = true;
        imagePlaceholder.sprite = screenshotSprite;
    }

    private IEnumerator DeleteScreenshot()
    {
        yield return new WaitForEndOfFrame();
        imagePlaceholder.enabled = false;
    } 

    
    void Awake()
    {
        directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
    }
}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.UnityEngine.UI;
using System.IO;

public class FlashView : MonoBehaviour
{
    [SerializeField] Image imagePlaceholder;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
        StartCoroutine (TakeScreenshotAndShow());
        }
    }


    private IEnumerator TakeScreenshotAndShow()
    {
        yield return new WaitForEndOfFrame();

        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();

        Texture2D newScreenshot = new Texture2D(screenshot.width, screenshot.height, TextureFormat.RGB24, false);
        newScreenshot. SetPixels (screenshot.GetPixels());
        newScreenshot. Apply();

        Destroy(screenshot);

        Sprite screenshotSprite = Sprite.Create(newScreenshot, new Rect(0, 0, newScreenshot.width, newScreenshot.height)
            , new Vector2(0.5f, 0.5f));

        whereToShowScreenshot.enabled = true;
        whereToShowScreenshot. sprite = screenshotSprite;
    }
}
*/