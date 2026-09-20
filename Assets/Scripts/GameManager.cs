using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // singleton: así las células le avisan directo sin tener que buscar el GameManager en la escena
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
    public Text textoRonda;
    public Text textoPuntaje;

    private float tiempoRestante;
    private int numeroRonda = 1;
    // las guardamos para reusarlas cada ronda en vez de crear células nuevas
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
            // esta primera llamada solo les da su color inicial, todavía no hay nada que premiar
            nueva.NuevaRonda();
        }

        ActualizarUI();
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

        // cada ronda es un paso de aprendizaje: las células reciben su punto y eligen color nuevo
        foreach (Celula celula in celulas)
        {
            celula.NuevaRonda();
        }

        Debug.Log("Nueva ronda: " + numeroRonda);
    }

    void ActualizarUI()
    {
        // los textos son opcionales, si no los arrastras en el Inspector no pasa nada
        if (textoTiempo != null)
        {
            // Ceil para que cuente 10...1 y no 9...0
            textoTiempo.text = Mathf.CeilToInt(tiempoRestante).ToString();
        }

        if (textoRonda != null)
        {
            textoRonda.text = "Ronda: " + numeroRonda;
        }

        if (textoPuntaje != null)
        {
            textoPuntaje.text = "Score: " + celulasEliminadas;
        }
    }

    // lo llama cada Celula cuando le dan clic
    public void RegistrarEliminacion()
    {
        celulasEliminadas++;
        Debug.Log("Células eliminadas: " + celulasEliminadas);
    }
}
