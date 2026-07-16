using UnityEngine;

public class EngineSoundController : MonoBehaviour
{
    [Header("References")]
    public AudioSource engineAudio;

    private float pausedTime = 6f;

    private void Start()
    {
        pausedTime = 10f;

        if (engineAudio != null)
        {
            engineAudio.Stop();
        }
    }

    private void Update()
    {
        if (engineAudio == null)
            return;

        // W pressed
        if (Input.GetKeyDown(KeyCode.Z))
        {
            engineAudio.time = pausedTime;
            engineAudio.Play();
        }

        // W released
        if (Input.GetKeyUp(KeyCode.Z))
        {
            pausedTime = engineAudio.time;
            engineAudio.Stop();
        }
    }
}
