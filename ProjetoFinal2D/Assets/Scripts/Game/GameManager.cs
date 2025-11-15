using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int pontos = 0;
    [HideInInspector] public bool deth;
    private bool hit;
    [HideInInspector] public int pressedButtonsCount;
    [SerializeField] private Botao[] buttons;
    [SerializeField] private Animator hudAnim;

    [SerializeField] bool imortal;

    [SerializeField] private Animator transition;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
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
        if ((hit || instant) && !imortal)
        {
            deth = true;
            Transition("Menu");
        }

        hit = true;
        hudAnim.SetBool("hit", hit);
    }

    public void Transition(string scene)
    {
        StartCoroutine(TransitionCoroutine(scene));
    }
    IEnumerator TransitionCoroutine(string scene)
    {
        transition.SetBool("go", true);
        yield return new WaitForSeconds(0.5f);
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
}
