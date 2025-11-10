using UnityEngine;

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
    [SerializeField] private float gravityDegrees;

    private float gravity;

    [Header("Push System")]
    [SerializeField] private Transform pushCheck;
    [SerializeField] private LayerMask pushLayer;
    [SerializeField] private float pushDistance;
    [SerializeField] private float pushVelocity;

    [Header("Parrot System")]
    [SerializeField] private Transform parrotTransform;
    [SerializeField] private float parrotMaxDistance;
    [SerializeField] private float parrotMinDistance;
    [SerializeField] private float parrotSpeed;
    [SerializeField] private Transform parrotTarget;

    private float currentParrotSpeed;
    private Transform parrotTransformTarget;
    private Animator parrotAnim;

    [Header("Attack System")]
    [SerializeField] private float radiusAttack;
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private LayerMask enemieLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        rigd = GetComponent<Rigidbody2D>();

        currentSpeed = speed;
        gravity = rigd.gravityScale;
        posicaoInicial = transform.position;  //pega posição inicial
        currentParrotSpeed = parrotSpeed;
        parrotTransformTarget = parrotTarget;

        parrotAnim = parrotTransform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        Push();
        FollowParrot();
        Attack();
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
            if (isground && pushRB == null) anim.SetInteger("transition", 1);
        }
        if (teclas < 0)
        {
            transform.eulerAngles = new Vector2(0, 180);
            if (isground && pushRB == null) anim.SetInteger("transition", 1);
        }
        if (teclas == 0 && isground)
        {
            if(pushRB == null) anim.SetInteger("transition", 0);
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

        if (!isground)
        {
            if(pushRB == null) anim.SetInteger("transition", 2);

            // sistema de pulo variavel
            if (Input.GetKey(KeyCode.Space) && rigd.linearVelocity.y > 0)
            {
                rigd.gravityScale = gravity * gravityDegrees;
            }
            else
            {
                rigd.gravityScale = gravity * (2 + gravityDegrees);
            }
        }
        else rigd.gravityScale = gravity;


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
            if (pushRB.linearVelocity.y == 0 && !pushCollider.isTrigger ) // empurrando algo
            {
                pushRB.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
                pushRB.linearVelocity = new Vector2(rigd.linearVelocity.x, pushRB.linearVelocityY);
                currentSpeed = speed * pushVelocity;
                parrotTransformTarget = pushRB.transform;
                anim.SetInteger("transition", 4);
            }
        }
        if (pushRB  != null && !ispushing) 
        {
            pushRB.constraints |= RigidbodyConstraints2D.FreezePositionX;
            pushRB.linearVelocity = Vector2.zero;
            pushRB = null;
            currentSpeed = speed;
            parrotTransformTarget = parrotTarget;
            anim.SetInteger("transition", 0);
        }
    }

    // sistema do papagaio
    void FollowParrot()
    {
        parrotTransform.position = Vector2.MoveTowards(parrotTransform.position, parrotTransformTarget.position, currentParrotSpeed*Time.deltaTime);
        float distance = Vector2.Distance(parrotTransform.position, parrotTransformTarget.position);
        if (distance > parrotMaxDistance) currentParrotSpeed = parrotSpeed * 2;
        else currentParrotSpeed = parrotSpeed;

        if (parrotTransform.position.x >= parrotTransformTarget.position.x && distance > parrotMinDistance) parrotTransform.eulerAngles = Vector3.up * 180;
        if(parrotTransform.position.x < parrotTransformTarget.position.x && distance > parrotMinDistance) parrotTransform.eulerAngles = Vector3.zero;

        parrotAnim.SetBool("Fly", !(distance < parrotMinDistance));

    }

    void Attack()
    {
        Collider2D hit = Physics2D.OverlapBox(attackOrigin.position + transform.right*(radiusAttack/2), new Vector2(radiusAttack, radiusAttack/2), 0, enemieLayer);

        if (Input.GetKeyDown(KeyCode.C) && hit)
        {
            Destroy(hit.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Morreu")
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<Player>().ReiniciarPosicao();
            gameManager.PerderVidas(1);
        }
    }
}