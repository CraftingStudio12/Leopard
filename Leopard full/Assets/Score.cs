using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Score : MonoBehaviour
{
    public float Distance;
    public Text text;
    public GameObject player;
    public Text textTwo;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Distance = player.transform.position.x;
        string textForm = Distance.ToString();
        text.text = textForm;
        textTwo.text = text.text;
    }
}
