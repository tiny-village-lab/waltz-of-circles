using UnityEngine;

public class Tutorial1 : MonoBehaviour
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
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
