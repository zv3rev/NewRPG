using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Information : MonoBehaviour
{
    public string offsetCoordinates;
    public int cost;
    public int actionPoints;

    private Text text;

    public void Start()
    {
        text = GetComponent<Text>(); 
    }

    public void UpdateInformation()
    {
        text.text = $"Offset coordinates: {offsetCoordinates} \r\n" +
                    $"Step cost: {cost}\r\n" +
                    $"Action point to reach: {actionPoints}";
    }

    public void ClearInformattion()
    {
        text.text = "";
    }
}
