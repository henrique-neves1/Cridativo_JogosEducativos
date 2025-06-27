using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

public class AudioVolumeSync : MonoBehaviour
{
    private AudioSource parentAudioSource;
    private List<AudioSource> childAudioSources = new List<AudioSource>();
    private List<VideoPlayer> videoPlayers = new List<VideoPlayer>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the AudioSource on this (parent) GameObject
        parentAudioSource = GetComponent<AudioSource>();

        if (parentAudioSource == null)
        {
            Debug.LogError("No AudioSource found on parent object.");
            return;
        }

        TraverseAndCollect(transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (parentAudioSource == null) return;

        float volume = parentAudioSource.volume;

        // Sync the volume to all children (excluding the parent)
        foreach (var audio in childAudioSources)
        {
            if (audio != null && audio != parentAudioSource)
                audio.volume = volume;
        }

        foreach (var vp in videoPlayers)
        {
            if (vp != null && vp.audioOutputMode == VideoAudioOutputMode.Direct)
            {
                ushort trackCount = (ushort)vp.audioTrackCount;
                for (ushort i = 0; i < trackCount; i++)
                {
                    vp.SetDirectAudioVolume(i, volume);
                }
            }
        }
    }

    private void TraverseAndCollect(Transform root)
    {
        foreach (Transform child in root)
        {
            // Look for AudioSource (even on disabled GameObjects)
            AudioSource audio = child.GetComponent<AudioSource>();
            if (audio != null)
                childAudioSources.Add(audio);

            // Look for VideoPlayer (even on disabled GameObjects)
            VideoPlayer vp = child.GetComponent<VideoPlayer>();
            if (vp != null)
                videoPlayers.Add(vp);

            // Recursive traversal
            TraverseAndCollect(child);
        }
    }
}
