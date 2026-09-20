using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource sfxAS;
    public AudioSource musicAS;
    // singleton: cualquier script puede llamar AudioManager.instance sin arrastrar referencias
    public static AudioManager instance;

    void Awake()
    {
        // solo el primero se queda como instancia, si aparece otro AudioManager se ignora
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySFX(AudioClip sfx, float volume = 1, float pitch = 1)
    {
        // ojo: el pitch es de todo el AudioSource, también afecta a los sonidos que ya están sonando
        sfxAS.pitch = pitch;
        // PlayOneShot deja que varios sonidos se solapen sin cortarse entre sí
        sfxAS.PlayOneShot(sfx, volume);
    }
}
