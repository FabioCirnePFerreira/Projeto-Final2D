using UnityEngine;

public class Player : MonoBehaviour
{
    //Pública: é acessivel no inspector;
    //Privada não é encontrada em nenhum outro lugar;
    public Vector2 posicaoInicial;
    public GameManager gameManager;

    public Animator anim;
    private Rigidbody2D rigd;
    public float speed;

    public float jumpForce = 5f;
    public bool isground;

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

        if (teclas > 0 && isground == true)
        {
            transform.eulerAngles = new Vector2(0, 0);
            anim.SetInteger("transition", 1);
        }
        if (teclas < 0 && isground == true)
        {
            transform.eulerAngles = new Vector2(0, 180);
            anim.SetInteger("transition", 1);
        }
        if(teclas == 0)
        {
            anim.SetInteger("transition", 0);
        }
    }
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isground == true)
        {
            rigd.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetInteger("transition", 2);
            isground = false;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "tagGround")
        {
            isground = true;
            Debug.Log("esta no chão");
        }
        if (collision.gameObject.tag == "Morreu")
        {
            Debug.Log("Morreu");
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<Player>().ReiniciarPosicao();
        }
    }
}
