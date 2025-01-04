using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;

    [System.Serializable]
    public class SoundGroup
    {
        public string actionName;
        public AudioClip[] clips;
    }

    public List<SoundGroup> soundGroups;

    private Dictionary<string, AudioClip[]> soundDictionary;

    private void Awake()
    {
        // Initialize the sound dictionary
        soundDictionary = new Dictionary<string, AudioClip[]>();

        foreach (SoundGroup group in soundGroups)
        {
            if (!soundDictionary.ContainsKey(group.actionName))
            {
                soundDictionary[group.actionName] = group.clips;
            }
        }
    }

    public void PlayRandomSound(string actionName)
    {
        if (soundDictionary.ContainsKey(actionName))
        {
            AudioClip[] clips = soundDictionary[actionName];
            if (clips.Length > 0)
            {
                int randomIndex = Random.Range(0, clips.Length);
                audioSource.PlayOneShot(clips[randomIndex]);
            }
        }
        else
        {
            Debug.LogWarning($"No sound group found for action: {actionName}");
        }
    }
}