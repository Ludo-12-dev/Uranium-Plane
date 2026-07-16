using UnityEngine;

public class Explodes : MonoBehaviour
{
    [Header("Target Objects")]
    public Transform avionTarget;              // Avion & Explosion/Avion
    public GameObject originalBoomAnimator;    // Avion & Explosion/BoomAnimator
    public GameObject boomAnimatorPrefab;      // BoomAnimator prefab to spawn
    public AudioSource ExplosionAudio;
    [Header("Explosion Settings")]
    public float explosionDistance = 550f;
    private float pausedTime = 14f;
    private bool hasExploded = false;

    public float startAudioTime = 14f;
    public float audioVolume = 1f;

    void Update()
    {
        if (hasExploded)
            return;

        if (avionTarget == null ||
            originalBoomAnimator == null ||
            boomAnimatorPrefab == null)
            return;

        // Wait until the original BoomAnimator is activated
        if (!originalBoomAnimator.activeInHierarchy)
            return;

        float distance = DistanceOnXZPlane(
            transform.position,
            avionTarget.position
        );

        if (distance <= explosionDistance)
        {
            Explode();
        }
    }

    float DistanceOnXZPlane(Vector3 a, Vector3 b)
    {
        Vector2 aXZ = new Vector2(a.x, a.z);
        Vector2 bXZ = new Vector2(b.x, b.z);

        return Vector2.Distance(aXZ, bXZ);
    }

    void Explode()
    {
        hasExploded = true;

        Vector3 spawnPos = transform.position;
        spawnPos.y = 0f;

        GameObject boom = Instantiate(
            boomAnimatorPrefab,
            spawnPos,
            Quaternion.Euler(0f, 90f, 0f)
        );
        boom.SetActive(true);
        
        PlayExplosionAudioAdditive(avionTarget.transform.position);

       

        // Deactivate Avion(1)
        gameObject.SetActive(false);
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