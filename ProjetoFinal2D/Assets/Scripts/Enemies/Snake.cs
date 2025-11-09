using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private LayerMask layerCheckPath;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float radiusArea;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        Run();
    }

    void Run()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, radiusArea, playerMask);
        RaycastHit2D rayCast = Physics2D.Raycast(rayOrigin.position, -rayOrigin.up, 0.5f, layerCheckPath);
        if (!player) transform.position += -transform.right * speed * Time.deltaTime;

        if (!rayCast)
        {
            transform.eulerAngles += Vector3.up * 180;
        }

        if (player)
        {
            anim.SetInteger("state", 1);
            if (player.transform.position.x < transform.position.x) transform.eulerAngles = Vector2.zero;
            else transform.eulerAngles = Vector2.up * 180;
        }
        else
        {
            anim.SetInteger("state", 0);
        }
    }
}
