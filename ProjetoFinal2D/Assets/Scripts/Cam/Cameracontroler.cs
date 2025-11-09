using UnityEngine;

public class CameraController : MonoBehaviour
{
    //public Transform player;
    //public float minX, maxX;
    //public float minY, maxY;
    //public float smoothTime = 0.3f;

    //void Update()
    //{
    //    if (player == null) return;

    //    Vector3 targetPosition = player.position + new Vector3(0, 0, -10);
    //    Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / smoothTime);

    //    newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
    //    newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

    //    transform.position = newPosition;
    //}

    [Header("Alvo")]
    [Tooltip("O Transform do jogador (player) que a câmera deve seguir.")]
    public Transform target;

    [Header("Controle de Suavização")]
    [Tooltip("Quão rápido a câmera se move em direção ao alvo. Valores maiores resultam em movimento mais rápido e menos 'liso'.")]
    public float smoothSpeed = 0.125f;

    [Header("Offset")]
    [Tooltip("Deslocamento fixo da câmera em relação ao centro do alvo.")]
    public Vector3 offset = new Vector3(0f, 2f, -10f);

    [Header("Dead Zone (Zona Morta)")]
    [Tooltip("Define a distância mínima (eixo X e Y) que o jogador precisa se afastar do centro da tela para que a câmera comece a seguir.")]
    public Vector2 deadZone = new Vector2(2f, 1.5f);

    // Posição para onde a câmera deve se mover (calculada a cada frame)
    private Vector3 desiredPosition;

    // O Update da Câmera deve ser sempre LateUpdate()
    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("O alvo (Target) da câmera não foi definido no Inspector!");
            return;
        }

        // 1. Calcula a posição desejada (posição do alvo + offset)
        desiredPosition = target.position + offset;

        // --- Lógica da Dead Zone ---

        // Calcula a diferença entre a posição da câmera e a posição desejada (alvo)
        Vector3 delta = desiredPosition - transform.position;
        Vector3 newPosition = transform.position;

        // Verifica a Dead Zone no eixo X
        if (Mathf.Abs(delta.x) > deadZone.x)
        {
            // Move no X se a diferença for maior que a Dead Zone
            newPosition.x = desiredPosition.x - Mathf.Sign(delta.x) * deadZone.x;
        }

        // Verifica a Dead Zone no eixo Y
        if (Mathf.Abs(delta.y) > deadZone.y)
        {
            // Move no Y se a diferença for maior que a Dead Zone
            newPosition.y = desiredPosition.y - Mathf.Sign(delta.y) * deadZone.y;
        }

        // Mantém o Z original do offset (geralmente -10 para 2D)
        newPosition.z = desiredPosition.z;


        // 2. Aplica a Suavização (Lerp)
        // Usa Vector3.Lerp para mover a câmera suavemente da posição atual para a nova posição calculada.
        transform.position = Vector3.Lerp(
            transform.position,     // Ponto de partida (Câmera atual)
            newPosition,            // Ponto de chegada (Alvo ajustado pela Dead Zone)
            smoothSpeed * Time.deltaTime // Fator de tempo para suavização
        );
    }
}
