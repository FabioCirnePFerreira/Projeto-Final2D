using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //GameManager desorganizado, resolver se der tempo :(

    [HideInInspector] public Player player;
    public static GameManager instance;
    public int pontos = 0;
    [HideInInspector] public bool deth;
    private bool hit;
    [HideInInspector] public bool shield;
    [HideInInspector] public int pressedButtonsCount;
    [SerializeField] private Botao[] buttons;
    [SerializeField] private Animator hudAnim;

    [SerializeField] bool imortal;

    [SerializeField] private Animator transition;

    [HideInInspector] public bool onDialogue;
    public Dialogue dialogue;
    private void Awake()
    {
        instance = this;
    }

    public void Addpontos(int qtd)     //Pontos
    {
        pontos += qtd;

        if (pontos <= 0)
        {
            pontos = 0;
        }
    }

    public void PerderVidas(bool instant)
    {
        if (!shield || instant)
        {
            if ((hit || instant) && !imortal)
            {
                deth = true;
                Transition("Menu");
            }
            hit = true;
            hudAnim.SetBool("hit", hit);
        }
        else LoseShield();
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
        StartCoroutine(TransitionCoroutine(scene));
    }
    IEnumerator TransitionCoroutine(string scene)
    {
        transition.SetBool("go", true);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(scene);
        yield return null;
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
        if (x >= buttons.Length)
        {
            Debug.Log("Ganhou");
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
}
