using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Vector3 offset;
    [SerializeField] public BoxCollider2D cameraBounds;

    private Transform player;

    public float smooth = 0.125f;

    Vector2 minBounds;
    Vector2 maxBounds;

    public void ShakeCamera(float duration, float magnitude, float noize)
    {
        StartCoroutine(ShakeCameraCor(duration, magnitude, noize));
    }

    
    private IEnumerator ShakeCameraCor(float duration, float magnitude, float noize)
    {
        float elapsed = 0f;
        Vector2 noizeStartPoint0 = Random.insideUnitCircle * noize;
        Vector2 noizeStartPoint1 = Random.insideUnitCircle * noize;

        while (elapsed < duration)
        {
            Vector2 currentNoizePoint0 = Vector2.Lerp(noizeStartPoint0, Vector2.zero, elapsed / duration);
            Vector2 currentNoizePoint1 = Vector2.Lerp(noizeStartPoint1, Vector2.zero, elapsed / duration);
            Vector2 cameraPostionDelta = new Vector2(Mathf.PerlinNoise(currentNoizePoint0.x, currentNoizePoint0.y), Mathf.PerlinNoise(currentNoizePoint1.x, currentNoizePoint1.y));
            cameraPostionDelta *= magnitude;

            transform.Translate(cameraPostionDelta);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void Start()
    {
        minBounds = cameraBounds.bounds.min + new Vector3(Camera.main.orthographicSize / Screen.height * Screen.width, Camera.main.orthographicSize, 0);
        maxBounds = cameraBounds.bounds.max - new Vector3(Camera.main.orthographicSize / Screen.height * Screen.width, Camera.main.orthographicSize, 0);
        player = GameObject.Find("Player").transform;
    }

    private void FixedUpdate()
    {
        float smooth1 = smooth / Time.fixedDeltaTime;
        Vector3 smoothPos = Vector3.Lerp(transform.position, player.position + offset, smooth1 * Time.fixedDeltaTime);
        transform.position = BoundCamera(smoothPos);
    }

    private Vector3 BoundCamera(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        
        return pos;
    }
}