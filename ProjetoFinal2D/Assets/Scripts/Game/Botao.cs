using UnityEngine;

public class Botao : MonoBehaviour
{
    [HideInInspector] public bool isPressed;
    [SerializeField] private LayerMask layer;
    [SerializeField] private Sprite sprites;
    private SpriteRenderer sprRender;
    [SerializeField] private AudioSource buttonAudio;

    void OnTriggerEnter2D(Collider2D colision)
    {
        if (colision.gameObject.layer == 6)
        {

            colision.GetComponent<Rigidbody2D>().constraints |= RigidbodyConstraints2D.FreezePosition;
            colision.transform.position = new Vector3(transform.position.x, colision.transform.position.y, 0);
            sprRender.sprite = sprites;
            isPressed = true;
            colision.GetComponent<BoxCollider2D>().isTrigger = true;
            GameManager.instance.VictoryCondition();

            buttonAudio.Play();
        }
    }

    void Start()
    {
        sprRender = gameObject.GetComponent<SpriteRenderer>();
    }
}
