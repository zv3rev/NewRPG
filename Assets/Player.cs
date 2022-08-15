using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Cell currentPlace;
    public Grid grid;

    [SerializeField] private Color baseColor;
    [SerializeField] private Color selectColor;

    public int stepsAmount = 5;

    public void MoveToCell()
    {
        Transform transform = GetComponent<Transform>();
        Vector2 destination = currentPlace.GetComponent<Transform>().position;
        transform.position = destination;
    }

    public void Select()
    {
        GetComponent<SpriteRenderer>().color = selectColor;
    }

    public void Unselect()
    {
        GetComponent<SpriteRenderer>().color = baseColor;
    }

    private void OnMouseDown()
    {
        if(stepsAmount>0)
        {
            grid.SetCurrentPlayer(this);
        }
    }
}
