using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Teleport : Button
{

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        GameManager.instance.TeleportModeOn();
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        GameManager.instance.TeleportModeOff();
    }
}
