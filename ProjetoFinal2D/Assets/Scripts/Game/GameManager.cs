using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int pontos = 0;
    public int vidas = 5;
    public int pressedButtonsCount;
    [SerializeField] private Botao[] buttons;

    public TextMeshProUGUI textVidas;
    public TextMeshProUGUI textPontos;

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

        textPontos.text = "Pontos: " + pontos;

        Debug.Log("Pontos: " + pontos);
    }

    public void PerderVidas(int vida)
    {
        vidas -= vida;
        Debug.Log("Vidas: " + vidas);
        GameObject player = GameObject.FindWithTag("Player");
        textVidas.text = "Vidas: " + vidas;
        player.GetComponent<Player>().ReiniciarPosicao();

        if (vidas <= 0)
        {
            Time.timeScale = 0;
            Debug.Log("Game Over.");
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
        if (x >= buttons.Length) 
        {
            Debug.Log("Ganhou");
        }
    } 
}
