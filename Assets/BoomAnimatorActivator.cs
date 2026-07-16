using UnityEngine;

public class BoomAnimatorLegiferinScript : MonoBehaviour
{
    [Header("References")]
    public AvionMovementPrevious avionMovement;
    public GameObject avionObject;
    public GameObject boomAnimatorObject;

    [Header("Ground Detection")]
    public float groundY = 0f;
    public float tolerance = 0.1f;

    [Header("Explosion Audio")]
    public AudioSource ExplosionAudio;
    public float startAudioTime = 14f;
    public float audioVolume = 1f;

    private bool hasBeenAirborne = false;
    private bool crashTriggered = false;

    private void Start()
    {
        if (boomAnimatorObject != null)
            boomAnimatorObject.SetActive(false);
    }

    private void Update()
    {
        if (avionMovement == null || avionObject == null || boomAnimatorObject == null)
            return;

        if (crashTriggered)
            return;

        if (avionMovement.airborne)
            hasBeenAirborne = true;

        if (!hasBeenAirborne)
            return;

        if (avionObject.transform.position.y <= groundY + tolerance)
        {
            crashTriggered = true;

            boomAnimatorObject.transform.position = avionObject.transform.position;
            boomAnimatorObject.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            boomAnimatorObject.SetActive(true);

            PlayExplosionAudioAdditive(avionObject.transform.position);

            avionObject.SetActive(false);
        }
    }

    public void PlayExplosionAudioAdditive(Vector3 position)
    {
        if (ExplosionAudio == null || ExplosionAudio.clip == null)
            return;

        GameObject audioObj = new GameObject("ExplosionAudio_Additive");
        audioObj.transform.position = position;

        AudioSource newSource = audioObj.AddComponent<AudioSource>();

        newSource.clip = ExplosionAudio.clip;
        newSource.volume = audioVolume;
        newSource.spatialBlend = ExplosionAudio.spatialBlend;
        newSource.outputAudioMixerGroup = ExplosionAudio.outputAudioMixerGroup;
        newSource.pitch = ExplosionAudio.pitch;
        newSource.loop = false;

        newSource.time = Mathf.Clamp(startAudioTime, 0f, ExplosionAudio.clip.length - 0.01f);
        newSource.Play();

        Destroy(audioObj, ExplosionAudio.clip.length - startAudioTime + 1f);
    }
}