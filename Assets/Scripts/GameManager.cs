using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia;

    [Header("Spawn de células")]
    public Celula prefabCelula;
    public int cantidadCelulas = 5;
    public float limiteX = 4f;
    public float limiteY = 2.5f;

    [Header("Contador de clics")]
    public int celulasEliminadas = 0;

    [Header("Temporizador de rondas")]
    public float duracionRonda = 10f;
    public Text textoTiempo;

    private float tiempoRestante;
    private int numeroRonda = 1;
    private List<Celula> celulas = new List<Celula>();

    void Awake()
    {
        Instancia = this;
    }

    void Start()
    {
        tiempoRestante = duracionRonda;

        for (int i = 0; i < cantidadCelulas; i++)
        {
            Vector3 posicion = new Vector3(
                Random.Range(-limiteX, limiteX),
                Random.Range(-limiteY, limiteY),
                0f
            );
            Celula nueva = Instantiate(prefabCelula, posicion, Quaternion.identity);
            celulas.Add(nueva);
            nueva.NuevaRonda();
        }
    }

    void Update()
    {
        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0f)
        {
            NuevaRonda();
        }

        ActualizarUI();
    }

    void NuevaRonda()
    {
        numeroRonda++;
        tiempoRestante = duracionRonda;

        foreach (Celula celula in celulas)
        {
            celula.NuevaRonda();
        }

        Debug.Log("Nueva ronda: " + numeroRonda);
    }

    void ActualizarUI()
    {
        if (textoTiempo != null)
        {
            textoTiempo.text = Mathf.CeilToInt(tiempoRestante).ToString();
        }
    }

    public void RegistrarEliminacion()
    {
        celulasEliminadas++;
        Debug.Log("Células eliminadas: " + celulasEliminadas);
    }
}
