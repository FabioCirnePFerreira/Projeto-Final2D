using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    //Pública: é acessivel no inspector;
    //Privada não é encontrada em nenhum outro lugar;
    public Vector2 posicaoInicial;
    public GameManager gameManager;

    [Header("Move System")]
    private Animator anim;
    private Rigidbody2D rigd;
    public float speed;
    private float currentSpeed;

    [Header("Jump System")]
    public float jumpForce = 5f;
    private bool isground;

    [SerializeField] private GameObject groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance;

    [Header("Push System")]
    [SerializeField] private Transform pushCheck;
    [SerializeField] private LayerMask pushLayer;
    [SerializeField] private float pushDistance;
    [SerializeField] private float pushVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSpeed = speed;
        anim = GetComponent<Animator>();
        rigd = GetComponent<Rigidbody2D>();
        posicaoInicial = transform.position;  //pega posição inicial
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        Push();
    }

    public void ReiniciarPosicao()
    {
        transform.position = posicaoInicial;
    }

    void Move()
    {
        float teclas = Input.GetAxis("Horizontal");
        rigd.linearVelocity = new Vector2(teclas * currentSpeed, rigd.linearVelocity.y);

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
        //Debug.Log(isground);
        Debug.DrawLine(groundCheck.transform.position, groundCheck.transform.position + Vector3.down * groundCheckDistance);

        Vector2 v = rigd.linearVelocity;
        if (Input.GetKeyDown(KeyCode.Space) && isground)
        {
            rigd.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        anim.SetFloat("velocitY", v.y);

        if (!isground) anim.SetInteger("transition", 2);


    }

    Rigidbody2D pushRB = null;
    BoxCollider2D pushCollider = null;
    void Push() 
    {
        RaycastHit2D ispushing = Physics2D.Raycast(pushCheck.position, transform.right, pushDistance, pushLayer);
        
        if (ispushing )
        {
            pushRB = ispushing.rigidbody;
            pushCollider = pushRB.GetComponent<BoxCollider2D>();
            if (pushRB.linearVelocity.y == 0 && !pushCollider.isTrigger )
            {
                pushRB.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
                pushRB.linearVelocity = new Vector2(rigd.linearVelocity.x, pushRB.linearVelocityY);
                currentSpeed = speed * pushVelocity;
            }
        }
        if (pushRB  != null && !ispushing) 
        {
            pushRB.constraints |= RigidbodyConstraints2D.FreezePositionX;
            pushRB.linearVelocity = Vector2.zero;
            pushRB = null;
            currentSpeed = speed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Morreu")
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<Player>().ReiniciarPosicao();
            gameManager.PerderVidas(1);
        }
    }
}