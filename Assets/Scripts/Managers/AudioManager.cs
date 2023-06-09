using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System;
using System.Runtime.InteropServices;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance { get; private set; }

    private FMOD.Studio.EventInstance eventInstance;

    public event Action Beat;

    public event Action Bar;

    private int lastMusicBar = 0;

    private int lastMusicBeat;

    // Variables that are modified in the callback need to be part of a seperate class.
    // This class needs to be 'blittable' otherwise it can't be pinned in memory.
    [StructLayout(LayoutKind.Sequential)]
    class TimelineInfo
    {
        public int currentMusicBar;
        public int currentMusicBeat;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }

    TimelineInfo timelineInfo;
    GCHandle timelineHandle;

    FMOD.Studio.EVENT_CALLBACK beatCallback;
    FMOD.Studio.EventInstance musicInstance;

    private void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }
        instance = this;
    }

    void Start()
    {
        LoadMusicInstance("event:/Music/Kicks");
    }

    private void LoadMusicInstance(string name)
    {
        timelineInfo = new TimelineInfo();

        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
        // Pin the class that will store the data modified during the callback
        timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);

        musicInstance = FMODUnity.RuntimeManager.CreateInstance(name);

        // Pass the object through the userdata of the instance
        musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));

        musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
        musicInstance.start();
    }

    public void Stop()
    {
        musicInstance.setUserData(IntPtr.Zero);
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
        timelineHandle.Free();
    }

    void Update()
    {
        if (lastMusicBar != timelineInfo.currentMusicBar) {
            lastMusicBar = timelineInfo.currentMusicBar;
            Bar?.Invoke();
        }

        if (lastMusicBeat != timelineInfo.currentMusicBeat) {
            lastMusicBeat = timelineInfo.currentMusicBeat;
            Beat?.Invoke();
        }
    }

    public void PlayFxPlayerHit(Vector3 eventPosition)
    {
        RuntimeManager.PlayOneShot("event:/FX/Hit", eventPosition);
    }

    public void PlayFxLevelUp()
    {
        RuntimeManager.PlayOneShot("event:/FX/Progress");
    }

    public void PlayFxEnemyHit()
    {
        RuntimeManager.PlayOneShot("event:/FX/EnemyCrash");
    }

    public void PlayOneShot(EventReference eventReference, Vector3 eventPosition)
    {
        RuntimeManager.PlayOneShot(eventReference, eventPosition);
    }

    public void SetProgression(int index)
    {
        musicInstance.setParameterByName("Progression", index);
    }

    public void SetIntensity(int intensity) {
        musicInstance.setParameterByName("Intensity", intensity, true);
    }

    public void SetDanger(int danger) {
        musicInstance.setParameterByName("Danger", danger, true);
    }

    public void SetGameOn(int on) {
        musicInstance.setParameterByName("GameOn", on);
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance eventInstance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = eventInstance.getUserData(out timelineInfoPtr);
        
        if (result != FMOD.RESULT.OK) {
            Debug.LogError("Timeline Callback error: " + result);
        }

        else if (timelineInfoPtr != IntPtr.Zero) {
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentMusicBar = parameter.bar;
                        timelineInfo.currentMusicBeat = parameter.beat;
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.lastMarker = parameter.name;
                    }
                    break;
            }
        }

        return FMOD.RESULT.OK;
    }
}
