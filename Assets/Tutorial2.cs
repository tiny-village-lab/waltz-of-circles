using UnityEngine;

public class Tutorial2 : MonoBehaviour
{

    private CanvasGroup canvasGroup;

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>(); 
    }

    void Update()
    {
        if (player.transform.position.y != 0) {
            canvasGroup.alpha = 1.0f;
        }

        if (GameManager.instance.IsTeleportModeOn()) {
            canvasGroup.alpha = 0.0f;
            Destroy(gameObject);
        }
    }
}

