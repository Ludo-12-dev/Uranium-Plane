using UnityEngine;

public class UPASTTS : MonoBehaviour
{
    public AudioSource audioSource;

    void Awake()
    {
        // Keep this GameObject alive when loading new scenes
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Play the attached AudioSource if it exists and isn't already playing
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}