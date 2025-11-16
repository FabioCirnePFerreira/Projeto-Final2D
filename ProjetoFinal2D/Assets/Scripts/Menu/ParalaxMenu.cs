using UnityEngine;

public class ParalaxMenu : MonoBehaviour
{
    [SerializeField] float paralaxForce;
    [SerializeField] Transform bg;

    private Vector3 lastPositionMouse = Vector3.zero;
    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouseMove = mousePosition - lastPositionMouse;

        bg.position -= mouseMove*paralaxForce;

        lastPositionMouse = mousePosition;
    }
}
