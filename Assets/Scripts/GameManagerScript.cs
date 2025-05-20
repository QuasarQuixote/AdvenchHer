using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public GameObject winScreen;
    public float winDur = 5f;
    private float winTime;
    private bool winning = false;
    // Start is called before the first frame update
    void Start()
    {
        winScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (winning && Time.fixedTime > winTime + winDur) Lose();
    }

    public void Victory()
    {
        winScreen.SetActive(true);
        winning = true;
        winTime = Time.fixedTime;
    }

    public void Lose()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
