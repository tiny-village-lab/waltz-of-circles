using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * This object produces rythmic heartbeats
 */
public class Heartbeat : MonoBehaviour
{

    private float bpm = 132.0f;

    private double nextEventTime;

    public event Action Tick;

    public GameObject[] samplerSlots;

    // Start is called before the first frame update
    void Start()
    {
        nextEventTime = AudioSettings.dspTime + 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        double time = AudioSettings.dspTime;

        if (time + 0.4f > nextEventTime) {

            for (int i=0; i<samplerSlots.Length; i++) {
                SamplerSlot slot = samplerSlots[i].GetComponent<SamplerSlot>();
                
                // if should be played
                if (slot.HasToPlay()) {
                    slot.PlayScheduled(nextEventTime, nextEventTime + BpmToTime());
                }

                slot.NextStep();
            }
            
            Tick?.Invoke();
            nextEventTime += BpmToTime();
        }
    }

    private double BpmToTime()
    {
        return 60.0 / bpm;
    }
}
