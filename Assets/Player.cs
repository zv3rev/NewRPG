using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Cell currentPlace;
    public Grid grid;

    public int stepsAmount = 5;

    public void MoveToCell()
    {
        Transform transform = GetComponent<Transform>();
        Vector2 destination = currentPlace.GetComponent<Transform>().position;
        transform.position = destination;
    }
    public void DrawVariants()
    {
        if (currentPlace != null)
        {
            List<Cell>[] cells = grid.Reachable(currentPlace, stepsAmount);
            for (int i = 0; i < cells.Length; i++)
            {
                foreach (Cell cell in cells[i])
                {
                    cell.SetState(CellState.Active);
                }
            }
        }
    }
}
