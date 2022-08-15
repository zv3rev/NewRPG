using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControls : MonoBehaviour
{
    public Grid grid;
    public void NewTurnPressed()
    {
        grid.drawState = DrawState.Play;
        grid.GetCurrentPlayer().stepsAmount = 5;
    }

    public void BlockingPressed()
    {
        grid.drawState = DrawState.Block;
    }
}
