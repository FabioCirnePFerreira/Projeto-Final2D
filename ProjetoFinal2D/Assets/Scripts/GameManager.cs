using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int pontos = 0;
    public int vidas = 5;

    public TextMeshProUGUI textPontos;


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
        player.GetComponent<Player>().ReiniciarPosicao();

        if (vidas <= 0)
        {
            Time.timeScale = 0;
            Debug.Log("Game Over.");
        }
    }
}
