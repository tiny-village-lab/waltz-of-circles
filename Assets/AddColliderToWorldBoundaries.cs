using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddColliderToWorldBoundaries : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Camera cam = Camera.main;
        float height = cam.orthographicSize * 2;
        float width = height * cam.aspect;

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.transform.position = Vector3.zero;
        boxCollider.size = new Vector2(width, height);
    }
}
