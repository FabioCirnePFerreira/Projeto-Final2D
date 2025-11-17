using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    //GameManager desorganizado, resolver se der tempo :(

    [SerializeField] private GameObject hud;
    [HideInInspector] public Player player;
    public static GameManager instance;
    public int pontos = 0;
    [HideInInspector] public bool deth;
    private bool hit;
    [HideInInspector] public bool shield;
    [HideInInspector] public int pressedButtonsCount;
    [SerializeField] private Botao[] buttons;
    [SerializeField] private GameObject[] pedras;
    private List<Vector3> pedrasInitalPos = new List<Vector3>();
    [SerializeField] private Animator hudAnim;

    [SerializeField] bool imortal;

    [SerializeField] private Animator transition;

    [HideInInspector] public bool onDialogue;
    public Dialogue dialogue;

    [SerializeField] GameObject transitionDeth;
    [SerializeField] Animator doorAnim;
    [SerializeField] private PlayableDirector openDoorCutSceane;
    [SerializeField] private PlayableDirector cutssceanFinal;

    private void Awake()
    {
        instance = this;

        foreach(GameObject pedra in pedras)
        {
            pedrasInitalPos.Add(pedra.transform.position);
        }
    }

    public void Addpontos(int qtd)     //Pontos
    {
        pontos += qtd;

        if (pontos <= 0)
        {
            pontos = 0;
        }
    }

    public void PerderVidas(bool instant, SpriteRenderer playerSPR)
    {
        if (!deth)
        {
            if (!shield || instant)
            {
                if ((hit || instant) && !imortal)
                {
                    deth = true;
                    PlayerDead(playerSPR);
                }
                hit = true;
                hudAnim.SetBool("hit", hit);
            }
            else LoseShield();
        }
    }
    public void GainShield()
    {
        shield = true;
        hudAnim.SetBool("shield", true);
    }
    public void LoseShield()
    {
        shield = false;
        hudAnim.SetBool("shield", false);
    }

    public void Transition(string scene)
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }

        StartCoroutine(TransitionCoroutine(scene));
    }
    IEnumerator TransitionCoroutine(string scene)
    {
        transition.SetBool("go", true);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(scene);
        yield return null;
    }

    public void ResetRocks()
    {
        List<GameObject> lockedRocks = new List<GameObject>();
        foreach(Botao btn in buttons)
        {
            if (btn.pedra != null)
            {
                lockedRocks.Add(btn.pedra);
            }
        }

        int x = 0;
        foreach (GameObject pedra in pedras)
        {
            if (!lockedRocks.Contains(pedra)) pedra.transform.position = pedrasInitalPos[x];
            x++;
        }

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

    public void VictoryCondition()
    {
        int x = 0;
        foreach (Botao btn in buttons)
        {
            if (btn.isPressed)
            {
                x += 1;
                //Debug.Log("Pressionou");
                //Debug.Log(buttons.Length);
            }
        }
        if (x >= buttons.Length && !doorAnim.GetBool("open"))
        {
            doorAnim.SetBool("open", true);
            openDoorCutSceane.Play();
        }
    }

    public void PauseGame(GameObject menu)
    {
        menu.SetActive(true);
        Time.timeScale = 0;
    }
    public void Unpause(GameObject menu)
    {
        menu.SetActive(false);
        Time.timeScale = 1;
    }

    public void PlayerDead(SpriteRenderer playeSpr)
    {
        hud.SetActive(false);
        playeSpr.sortingOrder = 20;
        StartCoroutine(PlayerDeadCoroutine());
    }

    IEnumerator PlayerDeadCoroutine()
    {
        Time.timeScale = 0.3f;
        transitionDeth.SetActive(true);
        yield return new WaitForSeconds(1);
        Time.timeScale = 1;
        Transition("DethSceane");
        yield return null;
    }


    public void CutsceaneFinal()
    {
        cutssceanFinal.Play();
        StartCoroutine(final());
    }

    IEnumerator final()
    {
        yield return new WaitForSeconds(25);
        Transition("Menu");
    }
}
