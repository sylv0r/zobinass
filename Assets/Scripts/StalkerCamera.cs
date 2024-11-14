using UnityEngine;
using System.Collections;

public class StalkerCamera: MonoBehaviour
{
    [Header("References")]
    public Transform target;
    
    private float _timer;
    
    private void LateUpdate()
    {
        transform.LookAt(target);
        _timer += Time.deltaTime;
        if (_timer >= 5f)
        {
            StartCoroutine(MoveCamera());
            _timer = 0f;
        }
        
    }
    
    private IEnumerator MoveCamera()
    {
        // use Random.onUnitSphere to get a random position on the surface of a sphere with radius 1 and center at target.position 
        transform.position = GetRandomPosition();
        while (!Physics.Linecast(transform.position, target.position))
        { 
            transform.position = GetRandomPosition();
        }
        yield return null;
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
