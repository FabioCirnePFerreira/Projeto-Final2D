using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
public class Player : MonoBehaviour
{
    //Pública: é acessivel no inspector;
    //Privada não é encontrada em nenhum outro lugar;
    public Vector2 posicaoInicial;
    private GameManager gameManager;
    [SerializeField] Transform camFollow;

    [Header("Move System")]
    private Animator anim;
    private Rigidbody2D rigd;
    [SerializeField] private AudioSource walkSound;

    public float speed;
    private float currentSpeed;

    [Header("Jump System")]
    public float jumpForce = 5f;
    private bool isground;

    [SerializeField] private GameObject groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float gravityDegrees;
    [SerializeField] AudioSource jumpSound;
    [SerializeField] AudioSource landSound;

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
    [SerializeField] private float attackDelay;
    private bool attacking;

    [Header("Damage System")]
    [SerializeField] private float knockBack = 5;
    [SerializeField] private float knockBackTime = 0.5f;
    [SerializeField] private Color damageColorSprite;
    [SerializeField] private Animator camAnim;
    private SpriteRenderer spr;
    private bool onKnockBack;

    [Header("HookSystem")]
    [SerializeField] private float hookRadius;
    [SerializeField] private float hookGravity;
    [SerializeField] private float hookSpeed;
    private bool isOnHook;
    private Transform hookTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        rigd = GetComponent<Rigidbody2D>();
        spr = GetComponent <SpriteRenderer>();

        gameManager = GameManager.instance;
        currentSpeed = speed;
        gravity = rigd.gravityScale;
        posicaoInicial = transform.position;  //pega posição inicial
        currentParrotSpeed = parrotSpeed;
        parrotTransformTarget = parrotTarget;

        parrotAnim = parrotTransform.GetComponent<Animator>();


        gameManager.player = gameObject.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!attacking && !gameManager.onDialogue && !isOnHook)
        {
            Move();
            Jump();
        }
        Push();
        FollowParrot();
        Attack();

        if (isOnHook) OnHook();
    }

    //====================================REINICIA POSIÇÃO==========================================
    public void ReiniciarPosicao()
    {
        transform.position = posicaoInicial;
    }

    //===================================SISTEMA DE MOVIMENTO=======================================
    void Move()
    {
        if (onKnockBack) return;

        float teclas = Input.GetAxis("Horizontal");
        rigd.linearVelocity = new Vector2(teclas * currentSpeed, rigd.linearVelocity.y);

        if (teclas > 0)
        {
            walkSound.Play();
            transform.eulerAngles = new Vector2(0, 0);
            if (isground && pushRB == null) anim.SetInteger("transition", 1);
        }
        if (teclas < 0)
        {
            walkSound.Play();
            transform.eulerAngles = new Vector2(0, 180);
            if (isground && pushRB == null) anim.SetInteger("transition", 1);
        }
        if (teclas == 0 && isground)
        {
            walkSound.Stop();
            if(pushRB == null) anim.SetInteger("transition", 0);
        }
    }

    //=======================================SISTEMA DE PULO=======================================
    void Jump()
    {
        isground = Physics2D.Raycast(groundCheck.transform.position, Vector2.down, groundCheckDistance, groundLayer);
        //Debug.Log(isground);
        Debug.DrawLine(groundCheck.transform.position, groundCheck.transform.position + Vector3.down * groundCheckDistance);

        Vector2 v = rigd.linearVelocity;
        if (Input.GetKeyDown(KeyCode.Space) && isground)
        {
            rigd.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpSound.Play();
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

    //===================================SISTEMA DE EMPURRAR================================
    Rigidbody2D pushRB = null;
    BoxCollider2D pushCollider = null;
    AudioSource pushAudio = null;
    void Push() 
    {
        RaycastHit2D ispushing = Physics2D.Raycast(pushCheck.position, transform.right, pushDistance, pushLayer);

        if (ispushing )
        {
            pushRB = ispushing.rigidbody;
            pushCollider = pushRB.GetComponent<BoxCollider2D>();
            pushAudio = pushRB.GetComponent<AudioSource>();
            if (pushRB.linearVelocity.y == 0 && !pushCollider.isTrigger  && rigd.linearVelocity.x !=0) // empurrando algo
            {
                pushRB.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
                pushRB.linearVelocity = new Vector2(rigd.linearVelocity.x, pushRB.linearVelocityY);
                currentSpeed = speed * pushVelocity;
                parrotTransformTarget = pushRB.transform;
                anim.SetInteger("transition", 4);
                camAnim.SetBool("shake", true);

                if (!pushAudio.isPlaying)
                {
                    pushAudio.Play();
                }
            }
        }
        if (pushRB  != null && !ispushing) 
        {
            camAnim.SetBool("shake", false);
            pushRB.constraints |= RigidbodyConstraints2D.FreezePositionX;
            pushRB.linearVelocity = Vector2.zero;
            pushRB = null;
            currentSpeed = speed;
            parrotTransformTarget = parrotTarget;
            pushAudio.Stop();
        }
    }

    // =========================SISTEMA DO PAPAGUAIO==============================
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

    GameObject advice;
    public void ResetParrot()
    {
        parrotTransformTarget = parrotTarget;
        camAnim.GetComponent<CinemachineCamera>().Target.TrackingTarget = camFollow;
        Destroy(advice);
    }

    //=========================SISTEMA DE ATAQUE==============================
    void Attack()
    {
        Collider2D hit = Physics2D.OverlapBox(attackOrigin.position + transform.right * (radiusAttack / 2), new Vector2(radiusAttack, radiusAttack / 2), 0, enemieLayer);
        if (Input.GetKeyDown(KeyCode.C) && !attacking && isground)
        {
            rigd.linearVelocity = Vector2.zero;
            anim.SetInteger("transition", 3);
            attacking = true;
            StartCoroutine(AttackCoroutine(attackDelay, hit));
        }
    }
    IEnumerator AttackCoroutine(float duration,Collider2D obj)
    {
        yield return new WaitForSeconds(duration);
        if(obj) Destroy(obj.gameObject);
        attacking = false;
    }

    //=========================SISTEM DE KNOCKBACK=============================
    void KnockBack(Transform knockBackOrigin)
    {
        Vector2 dir = (knockBackOrigin.position - transform.position).normalized;// CALCULA O VETOR DA DIREÇÃO ENTRE O PLAYER E A ORIGEM DO KNOCKBACK
        rigd.linearVelocity = Vector2.zero;
        rigd.linearVelocity = (-dir + Vector2.up/2) * knockBack;
        onKnockBack = true;
        spr.color = damageColorSprite;
        camAnim.SetBool("shake", true);
        StartCoroutine(KnockBackCoroutine(knockBackTime));
    }
    IEnumerator KnockBackCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        onKnockBack = false;
        camAnim.SetBool("shake", false);
        spr.color = Color.white;
    }

    //=========================SISTEMA DE GANCHOS==============================

    void OnHook() 
    {
        rigd.gravityScale = 0;

        Vector3 vertice = (hookTransform.position + Vector3.down * radiusAttack);
        Vector3 verticeDir = vertice - transform.position;
        Vector2 movement = verticeDir * hookGravity;

        rigd.linearVelocity += movement;


        Vector3 dir = (transform.position - hookTransform.position).normalized;
        transform.position = hookTransform.position + dir * hookRadius;
    }

    //====================SISTEMA DE TRIGGER PARA A MORTE======================
    private void OnTriggerEnter2D(Collider2D collision) //  É PRECISO CALCULAR O LOG DE BASE 2 DO VALOR DA LAYER POR CONTA DA INTERPRETAÇÃO BINÁRIA DO LAYER MASK
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            KnockBack(collision.gameObject.transform);
            gameManager.PerderVidas(false);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("instantDeth"))
        {
            gameManager.PerderVidas(true);
        }

        if(collision.gameObject.layer == LayerMask.NameToLayer("Advice"))
        {
            walkSound.Stop();
            advice = collision.gameObject;
            gameManager.onDialogue = true;
            parrotTransformTarget = collision.transform;
            DialogueText dialogue = collision.GetComponent<DialogueText>();
            gameManager.dialogue.StartDialogue(dialogue.dialogue, dialogue.falas, dialogue.name_);
            anim.SetInteger("transition", 0);
            rigd.linearVelocity = Vector2.zero;
            camAnim.GetComponent<CinemachineCamera>().Target.TrackingTarget = parrotTransform;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 14 && Input.GetKeyDown(KeyCode.C))
        {
            isOnHook = true;
            hookTransform = collision.gameObject.transform;
            rigd.linearVelocity = Vector2.zero;
        }
    }
}