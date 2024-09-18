using NaughtyAttributes;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip track;

    [ReadOnly][SerializeField] private float secondsLeft = 0;
    [SerializeField] private float maxWaitFactor = 3;
    [SerializeField] private float maxStartWaitSeconds = 2;

    private void Start()
    {
        secondsLeft = Random.value * maxStartWaitSeconds;
    }

    private void Update()
    {
        secondsLeft -= Time.deltaTime;
        if (secondsLeft <= 0)
        {
            secondsLeft = 0;
            StartMusic();
        }
    }

    private void StartMusic()
    {
        audioSource.PlayOneShot(track);
        secondsLeft = track.length * ((Random.value + 1) * maxWaitFactor);
    }
}