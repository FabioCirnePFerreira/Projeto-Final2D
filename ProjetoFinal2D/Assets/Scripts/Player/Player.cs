using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
public class Player : MonoBehaviour
{
    //Pública: é acessivel no inspector;
    //Privada não é encontrada em nenhum outro lugar;
    public Vector2 posicaoInicial;
    private GameManager gameManager;
    [SerializeField] Transform camFollow;

    [Header("Move System")]
    private bool blockMove;
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
    [SerializeField] private GameObject bloodEffect;
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
    [SerializeField] private AudioSource dethSound;
    [SerializeField] private AudioSource levelSound;
    private SpriteRenderer spr;
    private bool onKnockBack;

    [Header("HookSystem")]
    [SerializeField] private float hookRadius;
    [SerializeField] private float hookGravity;
    [SerializeField] private float hookSpeed;
    [SerializeField] private float hookForceImpulse;
    [SerializeField] private float maxHigh;
    private bool isOnHook;
    private Transform hookTransform;

    [SerializeField] LayerMask doorLayer;

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

    void Update()
    {
        isground = Physics2D.Raycast(groundCheck.transform.position, Vector2.down, groundCheckDistance, groundLayer);
        if (!gameManager.deth)
        {
            if (!attacking && !gameManager.onDialogue && !isOnHook && !blockMove)
            {
                Move();
                Jump();
            }
            Push();
            FollowParrot();
            Attack();

            if (isOnHook) OnHook();
        }
        if(gameManager.deth && !dethSound.isPlaying)
        {
            anim.SetInteger("transition", 5);
            walkSound.Stop();
            dethSound.Play();
            rigd.linearVelocity = Vector2.zero;
            levelSound.Stop();
        }


        RaycastHit2D interactDoor = Physics2D.CircleCast(transform.position, 5, Vector2.zero, 0, doorLayer);

        if(interactDoor && interactDoor.collider.GetComponent<Animator>().GetBool("open") && Input.GetKeyDown(KeyCode.E))
        {
            levelSound.Stop();
            gameManager.CutsceaneFinal();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gameManager.ResetRocks();
        }
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
            if(!walkSound.isPlaying) walkSound.Play();
            transform.eulerAngles = new Vector2(0, 0);
            if (isground && pushRB == null) anim.SetInteger("transition", 1);
        }
        if (teclas < 0)
        {
            if(!walkSound.isPlaying) walkSound.Play();
            transform.eulerAngles = new Vector2(0, 180);
            if (isground && pushRB == null) anim.SetInteger("transition", 1);
        }
        if (teclas == 0 && isground)
        {
            if(pushRB == null) anim.SetInteger("transition", 0);
        }

        if(teclas ==0 || !isground)
        {
            walkSound.Stop();
        }
    }

    //=======================================SISTEMA DE PULO=======================================
    void Jump()
    {
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
                pushRB.linearVelocity = new Vector2(rigd.linearVelocity.x, pushRB.linearVelocity.y);
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
        if (obj)
        {
            Instantiate(bloodEffect, obj.transform.position, Quaternion.identity);
            Destroy(obj.gameObject);
        }
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

    float targetHigh;
    float initicalSide;
    void OnHook() 
    {
        anim.SetInteger("transition", 6);
        rigd.gravityScale = 0;
        Vector3 player = transform.position;
        Vector3 hook = hookTransform.position;
        Vector3 vertice = (hook + Vector3.down * radiusAttack);
        Vector3 verticeDir = vertice - player;
        float high = hook.y - player.y;
        float actualSide = player.x - hook.x;

        if(rigd.linearVelocity == Vector2.zero)
        {
            targetHigh = high + hookGravity;
            initicalSide = actualSide;
        }

        if(initicalSide > 0)
        {
            transform.localEulerAngles = new Vector3(0, 180);

            anim.SetFloat("swing", -actualSide/initicalSide);

            rigd.AddForce(Vector2.left * hookSpeed);
            if (Input.GetKeyDown(KeyCode.A))
            {
                rigd.AddForce(Vector2.left * hookForceImpulse, ForceMode2D.Impulse);
                targetHigh -= hookSpeed;
            }
            if (high <= targetHigh && initicalSide / actualSide < 0)
            {
                rigd.linearVelocity = Vector2.zero;
            }
        }
        if(initicalSide < 0)
        {
            transform.localEulerAngles = new Vector3(0, 0);

            anim.SetFloat("swing", -actualSide / initicalSide);

            rigd.AddForce(Vector2.right * hookSpeed);
            if (Input.GetKeyDown(KeyCode.D))
            {
                rigd.AddForce(Vector2.right * hookForceImpulse, ForceMode2D.Impulse);
                targetHigh -=hookSpeed;
            }
            if (high <= targetHigh && initicalSide / actualSide < 0)
            {
                rigd.linearVelocity = Vector2.zero;
            }
        }

        if (targetHigh < maxHigh) targetHigh = maxHigh;


        Vector3 dir = (transform.position - hookTransform.position).normalized;
        transform.position = hookTransform.position + dir * hookRadius;




        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetInteger("transition", 0);
            rigd.gravityScale = 1;
            rigd.AddForce(rigd.linearVelocity + Vector2.up*rigd.linearVelocity.x);
            isOnHook = false;
            StartCoroutine(WaitToGround());
            blockMove = true;
        }
    }

    IEnumerator WaitToGround()
    {
        yield return new WaitUntil(() => isground);
        blockMove = false;
    }

    //====================SISTEMA DE TRIGGER PARA A MORTE======================
    private void OnTriggerEnter2D(Collider2D collision) //  É PRECISO CALCULAR O LOG DE BASE 2 DO VALOR DA LAYER POR CONTA DA INTERPRETAÇÃO BINÁRIA DO LAYER MASK
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            KnockBack(collision.gameObject.transform);
            gameManager.PerderVidas(false, spr);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("instantDeth"))
        {
            gameManager.PerderVidas(true, spr);
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
            walkSound.Stop();
        }

        if(collision.gameObject.layer == 16)
        {
            collision.GetComponent<PlayableDirector>().Play();
        }
    }
}