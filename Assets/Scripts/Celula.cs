using UnityEngine;

public class Celula : MonoBehaviour
{
    [Header("Parámetros de la célula")]
    public Color color = Color.white;
    public float tamano = 1f;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        AplicarParametros();
    }

    public void AplicarParametros()
    {
        spriteRenderer.color = color;
        transform.localScale = new Vector3(tamano, tamano, 1f);
    }
}
