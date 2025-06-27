using UnityEngine;

public class StopAudioSource : MonoBehaviour
{
    public AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StopAudioSource1()
    {
        audioSource.Stop();
    }
}
