using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameOver : MonoBehaviour
{
    public float timeLeft = 60;
    public Text text;
    public GameObject obje;
    public GameObject can;
    public GameObject player;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= 1 * Time.deltaTime;
        text.text = timeLeft.ToString();


        if(timeLeft <= 0)
        {
            can.SetActive(true);
            Destroy(player,0f);
        }

    }




}