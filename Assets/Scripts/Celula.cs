using System.Collections.Generic;
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
    public float epsilon = 0.3f;
    // el piso: aunque baje mucho, nunca deja de explorar del todo
    public float epsilonMinimo = 0.01f;
    [Range(0f, 1f)]
    public float factorDecaimiento = 0.85f;

    [Header("Eliminación de colores malos")]
    public int rondaMinimaAntesDeEliminar = 5;
    // qué tan por debajo del mejor tiene que quedar un color para que lo saquemos
    public float margenEliminacion = 2f;

    [Header("Efectos")]
    public AudioClip explosionSFX;
    public GameObject explosionEffect;

    // un puntaje por color: +1 si sobrevivió la ronda, -1 si le dieron clic
    private float[] puntajes;
    private bool[] colorActivo;
    private int colorActualIndice = 0;
    private bool fueDetectada = false;
    // en la primera ronda todavía no hay color previo al que darle puntos
    private bool haHechoPrimeraRonda = false;
    private int rondaActual = 0;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        puntajes = new float[coloresPosibles.Length];
        colorActivo = new bool[coloresPosibles.Length];
        for (int i = 0; i < colorActivo.Length; i++)
        {
            colorActivo[i] = true;
        }
    }

    public void AplicarParametros()
    {
        spriteRenderer.color = coloresPosibles[colorActualIndice];
        transform.localScale = new Vector3(tamano, tamano, 1f);
    }

    public void NuevaRonda()
    {
        // si llegó al final de la ronda sin que le dieran clic, su color se ganó un punto
        if (haHechoPrimeraRonda && !fueDetectada)
        {
            puntajes[colorActualIndice] += 1f;
        }

        fueDetectada = false;
        haHechoPrimeraRonda = true;
        rondaActual++;

        // al principio prueba mucho y con las rondas se va quedando con lo que sirve
        epsilon = Mathf.Max(epsilonMinimo, epsilon * factorDecaimiento);

        if (rondaActual >= rondaMinimaAntesDeEliminar)
        {
            EliminarColoresMalos();
        }

        colorActualIndice = ElegirColor();
        AplicarParametros();
        // si estaba oculta porque le dieron clic, aquí vuelve a aparecer con su color nuevo
        gameObject.SetActive(true);
    }

    // eliminación sucesiva: cada ronda saca los colores que quedaron muy atrás del mejor
    private void EliminarColoresMalos()
    {
        float mejor = MejorPuntajeActivo();

        int activos = 0;
        for (int i = 0; i < colorActivo.Length; i++)
        {
            if (colorActivo[i]) activos++;
        }

        for (int i = 0; i < puntajes.Length; i++)
        {
            // dejamos mínimo 2 colores, con uno solo ya no habría nada que comparar
            if (activos <= 2) break;

            if (colorActivo[i] && puntajes[i] < mejor - margenEliminacion)
            {
                colorActivo[i] = false;
                activos--;
            }
        }
    }

    private float MejorPuntajeActivo()
    {
        float mejor = float.MinValue;
        for (int i = 0; i < puntajes.Length; i++)
        {
            if (colorActivo[i] && puntajes[i] > mejor)
            {
                mejor = puntajes[i];
            }
        }
        return mejor;
    }

    private int ElegirColor()
    {
        // epsilon-greedy: a veces explora un color random, si no, se queda con el mejor que conoce
        if (Random.value < epsilon)
        {
            return ColorActivoAlAzar();
        }

        return MejorColorActivo();
    }

    private int MejorColorActivo()
    {
        float mejor = MejorPuntajeActivo();

        // si hay empate elige al azar entre los mejores, para que no gane siempre el primero
        List<int> candidatos = new List<int>();
        for (int i = 0; i < puntajes.Length; i++)
        {
            if (colorActivo[i] && puntajes[i] == mejor)
            {
                candidatos.Add(i);
            }
        }

        return candidatos[Random.Range(0, candidatos.Count)];
    }

    private int ColorActivoAlAzar()
    {
        List<int> activos = new List<int>();
        for (int i = 0; i < colorActivo.Length; i++)
        {
            if (colorActivo[i]) activos.Add(i);
        }

        return activos[Random.Range(0, activos.Count)];
    }

    void OnMouseDown()
    {
        fueDetectada = true;
        puntajes[colorActualIndice] -= 1f;

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        // revisamos que exista el AudioManager por si la escena no lo tiene y así no truena
        if (explosionSFX != null && AudioManager.instance != null)
        {
            // pitch random para que la explosión no suene idéntica cada vez
            AudioManager.instance.PlaySFX(explosionSFX, 1, Random.Range(0.5f, 1.2f));
        }

        GameManager.Instancia.RegistrarEliminacion();
        // la ocultamos en vez de destruirla: si no, pierde sus puntajes y NuevaRonda no la puede revivir
        gameObject.SetActive(false);
    }
}
