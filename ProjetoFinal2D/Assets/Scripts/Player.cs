using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    //Pública: é acessivel no inspector;
    //Privada não é encontrada em nenhum outro lugar;
    public Vector2 posicaoInicial;
    public GameManager gameManager;

    private Animator anim;
    private Rigidbody2D rigd;
    public float speed;

    public float jumpForce = 5f;
    private bool isground;

    [SerializeField] private GameObject groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        rigd = GetComponent<Rigidbody2D>();
        posicaoInicial = transform.position;  //pega posição inicial
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
    }

    public void ReiniciarPosicao()
    {
        transform.position = posicaoInicial;
    }

    void Move()
    {
        float teclas = Input.GetAxis("Horizontal");
        rigd.linearVelocity = new Vector2(teclas * speed, rigd.linearVelocity.y);

        if (teclas > 0)
        {
            transform.eulerAngles = new Vector2(0, 0);
            if (isground) anim.SetInteger("transition", 1);
        }
        if (teclas < 0)
        {
            transform.eulerAngles = new Vector2(0, 180);
            if (isground) anim.SetInteger("transition", 1);
        }
        if (teclas == 0 && isground)
        {
            anim.SetInteger("transition", 0);
        }
    }
    void Jump()
    {

        isground = Physics2D.Raycast(groundCheck.transform.position, Vector2.down, groundCheckDistance, groundLayer);
        Debug.Log(isground);
        Debug.DrawLine(groundCheck.transform.position, groundCheck.transform.position + Vector3.down * groundCheckDistance);

        Vector2 v = rigd.linearVelocity;
        if (Input.GetKeyDown(KeyCode.Space) && isground)
        {
            rigd.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        anim.SetFloat("velocitY", v.y);

        if (!isground) anim.SetInteger("transition", 2);


    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Morreu")
        {
            Debug.Log("Morreu");
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<Player>().ReiniciarPosicao();
        }
    }
}