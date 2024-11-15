using UnityEngine;
using System.Collections;

public class StalkerCamera: MonoBehaviour
{
    [Header("References")]
    private Transform target;
    private FlashScreen flashScreen;
 
    [Header("Audio")]
    public AudioSource audioShutter;
    
    [Header("Flashlight")]
    public float initialFlashIntensity = 14.8f;
    public float flashDuration = 0.5f;
    
    [Header("Camera")]
    public float pictureInterval = 23f;
    
    private float _timer;
    
    private Camera _camera;
    private Light _light;
    
    private void Start()
    {
        target = GameObject.Find("Player(Clone)").transform;
        flashScreen = GameObject.Find("ShowScreenshotHolder").GetComponent<FlashScreen>();
        _camera = GetComponent<Camera>();
        _light = GetComponent<Light>();
    }
    
    private void LateUpdate()
    {
        transform.LookAt(target);
        _timer += Time.deltaTime;
        if (_timer >= pictureInterval)
        {
            StartCoroutine(MoveCamera());
            _timer = 0f;
        }
        
    }
    
    private IEnumerator MoveCamera()
    {
        // use Random.onUnitSphere to get a random position on the surface of a sphere with radius 1 and center at target.position check if the line from the target to the random position intersects with any collider
        transform.position = GetRandomPosition();
        while (Physics.Linecast(target.position, transform.position))
        { 
            transform.position = GetRandomPosition();
        }
        
        yield return null;
        
        _light.intensity = initialFlashIntensity;
        audioShutter.enabled = true;
        StartCoroutine(CaptureCamera());
        StartCoroutine(FadeOutFlash());
    }
    
    private IEnumerator FadeOutFlash()
    {
        var elapsedTime = 0f;
        
        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            _light.intensity = Mathf.Lerp(initialFlashIntensity, 0f, elapsedTime / flashDuration);
            yield return null;
        }
        audioShutter.enabled = false;
    }
    
    private IEnumerator CaptureCamera() {
        yield return new WaitForEndOfFrame();
        _camera.enabled = true;
        var renderTexture = new RenderTexture(Screen.width, Screen.height, 16);
        var texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        var target = _camera.targetTexture;
        _camera.targetTexture = renderTexture;
        _camera.Render();
        _camera.targetTexture = target;
        
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture2D.Apply();
        RenderTexture.active = null;
        
        var screenshotSprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
        _camera.enabled = false;
        StartCoroutine(flashScreen.MovePhotoUp(screenshotSprite));
    }
    
    private Vector3 GetRandomPosition()
    {
        // only get top half of the sphere
        var randomDirection = Random.onUnitSphere;
        
        // only get the top half of the sphere
        if (randomDirection.y < 0)
        {
            randomDirection.y *= -1;
        }
        
        return target.position + randomDirection * 6;
    }
}
