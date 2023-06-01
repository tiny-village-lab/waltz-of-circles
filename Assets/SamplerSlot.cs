using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamplerSlot : MonoBehaviour
{

    public AudioClip clip;

    public bool[] sequence;

    private int index = 0;

    private AudioSource[] audioSources = new AudioSource[2];
    private int indexAudioSource = 0;

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i<audioSources.Length; i++) {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
            audioSources[i].clip = clip;
        }
    }

    // Play later at a given time
    public void PlayScheduled(double eventTime, double nextEventTime)
    {
        audioSources[indexAudioSource].PlayScheduled(eventTime);
        //audioSources[indexAudioSource].SetScheduledEndTime(nextEventTime);

        indexAudioSource = (indexAudioSource + 1) % audioSources.Length;
    }

    // Prepare next step
    public void NextStep()
    {
        index = (index + 1) % sequence.Length;
    }

    // Return yes if the step needs to be played
    public bool HasToPlay()
    {
        return sequence[index];
    }
}