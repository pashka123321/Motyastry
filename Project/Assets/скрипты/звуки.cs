using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] soundClips;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomSound()
    {
        if (soundClips.Length > 0)
        {
            int randomIndex = Random.Range(0, soundClips.Length);
            audioSource.PlayOneShot(soundClips[randomIndex]);
        }
        else
        {
            Debug.LogWarning("Sound clips array is empty!");
        }
    }
}
