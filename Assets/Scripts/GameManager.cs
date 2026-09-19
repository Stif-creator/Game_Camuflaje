using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia;

    [Header("Contador de clics")]
    public int celulasEliminadas = 0;

    [Header("Temporizador de rondas")]
    public float duracionRonda = 10f;
    public Text textoTiempo;

    private float tiempoRestante;
    private int numeroRonda = 1;

    void Awake()
    {
        Instancia = this;
    }

    void Start()
    {
        tiempoRestante = duracionRonda;
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
