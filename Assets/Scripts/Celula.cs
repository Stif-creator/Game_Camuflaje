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
    public float epsilonMinimo = 0.01f;
    [Range(0f, 1f)]
    public float factorDecaimiento = 0.85f;

    [Header("Eliminación de colores malos")]
    public int rondaMinimaAntesDeEliminar = 5;
    public float margenEliminacion = 2f;

    [Header("Efectos")]
    public AudioClip explosionSFX;
    public GameObject explosionEffect;

    private float[] puntajes;
    private bool[] colorActivo;
    private int colorActualIndice = 0;
    private bool fueDetectada = false;
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
        if (haHechoPrimeraRonda && !fueDetectada)
        {
            puntajes[colorActualIndice] += 1f;
        }

        fueDetectada = false;
        haHechoPrimeraRonda = true;
        rondaActual++;

        epsilon = Mathf.Max(epsilonMinimo, epsilon * factorDecaimiento);

        if (rondaActual >= rondaMinimaAntesDeEliminar)
        {
            EliminarColoresMalos();
        }

        colorActualIndice = ElegirColor();
        AplicarParametros();
        gameObject.SetActive(true);
    }

    private void EliminarColoresMalos()
    {
        float mejor = MejorPuntajeActivo();

        for (int i = 0; i < puntajes.Length; i++)
        {
            if (colorActivo[i] && puntajes[i] < mejor - margenEliminacion)
            {
                colorActivo[i] = false;
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
        if (Random.value < epsilon)
        {
            return ColorActivoAlAzar();
        }

        return MejorColorActivo();
    }

    private int MejorColorActivo()
    {
        float mejor = MejorPuntajeActivo();

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
        if (explosionSFX != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(explosionSFX, 1, Random.Range(0.5f, 1.2f));
        }

        GameManager.Instancia.RegistrarEliminacion();
        gameObject.SetActive(false);
    }
}
