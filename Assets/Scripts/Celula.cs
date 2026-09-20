using UnityEngine;

public class Celula : MonoBehaviour
{
    [Header("Parámetros de la célula")]
    public float tamano = 1f;

    [Header("Aprendizaje por refuerzo")]
    public Color[] coloresPosibles = new Color[]
    {
        Color.red, Color.green, Color.blue, Color.yellow, Color.white
    };
    [Range(0f, 1f)]
    public float epsilon = 0.2f;
    public float epsilonMinimo = 0.02f;
    [Range(0f, 1f)]
    public float factorDecaimiento = 0.98f;

    private float[] puntajes;
    private int colorActualIndice = 0;
    private bool fueDetectada = false;
    private bool haHechoPrimeraRonda = false;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        puntajes = new float[coloresPosibles.Length];
    }

    public void AplicarParametros()
    {
        spriteRenderer.color = coloresPosibles[colorActualIndice];
        transform.localScale = new Vector3(tamano, tamano, 1f);
    }

    public void NuevaRonda()
    {
        if (haHechoPrimeraRonda && !fueDetectada)
        {
            puntajes[colorActualIndice] += 1f;
        }

        fueDetectada = false;
        haHechoPrimeraRonda = true;

        epsilon = Mathf.Max(epsilonMinimo, epsilon * factorDecaimiento);

        colorActualIndice = ElegirColor();
        AplicarParametros();
        gameObject.SetActive(true);
    }

    private int ElegirColor()
    {
        if (Random.value < epsilon)
        {
            return Random.Range(0, coloresPosibles.Length);
        }

        int mejorIndice = 0;
        for (int i = 1; i < puntajes.Length; i++)
        {
            if (puntajes[i] > puntajes[mejorIndice])
            {
                mejorIndice = i;
            }
        }
        return mejorIndice;
    }

    void OnMouseDown()
    {
        fueDetectada = true;
        puntajes[colorActualIndice] -= 1f;
        GameManager.Instancia.RegistrarEliminacion();
        gameObject.SetActive(false);
    }
}
