using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTimer : MonoBehaviour
{

    public List<GameObject> slots;

    private int index = 4;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.Beat += SubstractOneSlot;
        AudioManager.instance.Beat += AddOneSlot;
        GameManager.instance.OnGhostModeOn += SubstractOneSlot;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i=0; i<slots.Count; i++) {
            slots[i].GetComponent<CanvasRenderer>().SetAlpha(
                i < index ? 1.0f : 0.2f
            );
        }
    }

    void SubstractOneSlot()
    {
        if (! GameManager.instance.IsGhostModeOn()) {
            return;
        }

        if (index == 0) {
            GameManager.instance.GhostModeOff();
            return;
        }

        index--;
    }

    void AddOneSlot()
    {
        if (GameManager.instance.IsGhostModeOn()) {
            return;
        }

        if (index == 5) {
            return;
        }

        index++;
    }
}
