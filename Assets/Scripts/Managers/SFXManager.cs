using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [SerializeField] private AudioSource SFXobj;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audiosource = Instantiate(SFXobj, spawnTransform.position, Quaternion.identity);

        audiosource.clip = audioClip;

        audiosource.volume = volume;

        audiosource.Play();

        float clipLength = audiosource.clip.length;

        Destroy(audiosource.gameObject, clipLength);
    }

    public AudioSource PlayLoopingSFX(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(SFXobj, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = true;

        audioSource.Play();

        return audioSource;
    }

    public void StopLoopingSFX(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            Destroy(audioSource.gameObject);
        }
    }
}
