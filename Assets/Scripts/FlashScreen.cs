using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashScreen : MonoBehaviour
{
    private Image imagePlaceholder;
    private Image imageBackground;
    [SerializeField] private float photoMoveDuration = 2f;
    [SerializeField] private float photoFadeDuration = 10f;
    [SerializeField] private float photoDisplayDuration = 10f;
    
    [Header("Audio")]
    public AudioSource audioPrintPolaroid;
    
    private RectTransform _imageRectTransform;
    private Image _imageBackground;
    
    private void Start()
    {
        imagePlaceholder =  GameObject.Find("ShowScreenshot").GetComponent<Image>();
        imageBackground = GameObject.Find("ShowScreenshotBackground").GetComponent<Image>();
        _imageRectTransform = GetComponent<RectTransform>();
        _imageBackground = GetComponent<Image>();
        DeleteScreenshot();
    }
    
    public IEnumerator MovePhotoUp(Sprite screenshotSprite)
    {
        imagePlaceholder.sprite = screenshotSprite;
        _imageBackground.enabled = true;
        imagePlaceholder.enabled = true;
        imageBackground.enabled = true;
        audioPrintPolaroid.enabled = true;
        var elapsedTime = 0f;
        imagePlaceholder.color = new Color(1, 1, 1, 0);
        while (elapsedTime < photoMoveDuration)
        {
            elapsedTime += Time.deltaTime;
            _imageRectTransform.anchoredPosition = new Vector2(0, Mathf.Lerp(-_imageRectTransform.rect.height, 0, elapsedTime / photoMoveDuration));
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
        Invoke(nameof(DeleteScreenshot), photoDisplayDuration);
    }

   
    
    private void DeleteScreenshot()
    {
        _imageBackground.enabled = false;
        imageBackground.enabled = false;
        imagePlaceholder.enabled = false;
    } 
}