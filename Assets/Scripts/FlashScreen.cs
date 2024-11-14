using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashScreen : MonoBehaviour
{
    [SerializeField] private Image imageBackground;
    [SerializeField] private RectTransform imageRectTransform;
    [SerializeField] private Image imagePlaceholder;
    [SerializeField] private Light flashLight;
    [SerializeField] private float initialFlashIntensity = 14.8f;
    [SerializeField] private float flashDuration = 0.5f;
    [SerializeField] private float photoMoveDuration = 2f;
    [SerializeField] private float photoFadeDuration = 10f;
    [SerializeField] private float photoDisplayDuration = 100f;
    
    [Header("Audio")]
    public AudioSource audioShutter;
    public AudioSource audioPrintPolaroid;
    
    private float _timer;
    private Material _stalkerMaterial;
    private Camera _stalkerCamera;
    private bool _hasTriggered;


    private void Update()
    {
        _timer += Time.deltaTime;
        
        if (_timer >= photoDisplayDuration)
        {
            _timer = 0f;
            _hasTriggered = false;
        }

        if (_timer < 1.5f && !_hasTriggered)
        {
            flashLight.intensity = initialFlashIntensity;
            audioShutter.enabled = true;
            StartCoroutine(CaptureCamera());
            StartCoroutine(FadeOutFlash());
            _hasTriggered = true;
            Invoke(nameof(DeleteScreenshot), photoDisplayDuration);
        }
    }
    
    private IEnumerator MovePhotoUp()
    {
        var elapsedTime = 0f;
        imagePlaceholder.color = new Color(1, 1, 1, 0);
        while (elapsedTime < photoMoveDuration)
        {
            elapsedTime += Time.deltaTime;
            imageRectTransform.anchoredPosition = new Vector2(0, Mathf.Lerp(-imageRectTransform.rect.height, 0, elapsedTime / photoMoveDuration));
            yield return null;
        }
        audioPrintPolaroid.enabled = false;
        StartCoroutine(FadeInPhoto());
    }

    private IEnumerator FadeInPhoto()
    {
        var elapsedTime = 0f;
        while (elapsedTime < photoFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            imagePlaceholder.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, elapsedTime / photoFadeDuration));
            yield return null;
        }
    }

    private IEnumerator FadeOutFlash()
    {
        var elapsedTime = 0f;
        
        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            flashLight.intensity = Mathf.Lerp(initialFlashIntensity, 0f, elapsedTime / flashDuration);
            yield return null;
        }
        audioShutter.enabled = false;
    }
    
    private IEnumerator CaptureCamera() {
        yield return new WaitForEndOfFrame();
        _stalkerCamera.enabled = true;
        var renderTexture = new RenderTexture(Screen.width, Screen.height, 16);
        var texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        var target = _stalkerCamera.targetTexture;
        _stalkerCamera.targetTexture = renderTexture;
        _stalkerCamera.Render();
        _stalkerCamera.targetTexture = target;
        
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture2D.Apply();
        RenderTexture.active = null;
        
        var screenshotSprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
        imagePlaceholder.sprite = screenshotSprite;
        imageBackground.enabled = true;
        imagePlaceholder.enabled = true;
        _stalkerCamera.enabled = false;
        audioPrintPolaroid.enabled = true;
        StartCoroutine(MovePhotoUp());
    }
    
    private void DeleteScreenshot()
    {
        imageBackground.enabled = false;
        imagePlaceholder.enabled = false;
    } 
    
    private void Awake()
    {
        _stalkerCamera = GameObject.Find("StalkerCamera").GetComponent<Camera>();
    }
}