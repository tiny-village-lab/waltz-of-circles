using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    public List<GameObject> hearts;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnGameOver += Deactivate;
    }

    public void UpdateHealthBar(int health)
    {

        if (GameManager.instance.GameIsOver()) {
            return;
        }

        float alpha = 1.0f;

        for (int i=0; i<hearts.Count; i++) {
            alpha = 1.0f;

            if (health < i) {
                alpha = 0.2f;
            }
            
            hearts[i].GetComponent<CanvasRenderer>().SetAlpha(alpha);
        }
    }

    public void Deactivate()
    {
        for (int i=0; i<hearts.Count; i++) {
            hearts[i].GetComponent<CanvasRenderer>().SetAlpha(0.2f);
        }
    }
}
