using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Valuechange : MonoBehaviour
{
    public Text text;
    public Text oldt;
    void Start()
    {





    }

    // Update is called once per frame
    void Update()
    {
        oldt.text = text.text;
    }
}
