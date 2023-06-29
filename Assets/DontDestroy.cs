using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{

    void Awake()
    {
        string tag = this.gameObject.tag;

        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        if (objects.Length > 1) {
            Destroy(this.gameObject);
        }
        
        DontDestroyOnLoad(this.gameObject);
    }
}
