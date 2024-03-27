using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public BirdController birdController;
    public TextMeshProUGUI textMeshProUGUI;


    // Update is called once per frame
    void Update()
    {
        textMeshProUGUI.text = birdController.Points.ToString();
    }
}
