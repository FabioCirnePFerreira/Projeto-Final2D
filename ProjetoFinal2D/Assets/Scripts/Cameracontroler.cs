using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float minX, maxX;
    public float minY, maxY;
    public float smoothTime = 0.3f;

    void Update()
    {
        if (player == null) return;

        Vector3 targetPosition = player.position + new Vector3(0, 0, -10);
        Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / smoothTime);

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        transform.position = newPosition;
    }
}
