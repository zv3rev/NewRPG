using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum CellState
{
    Active, Inactive, Blocked
}

public class Cell : MonoBehaviour
{
    private Grid grid;
    public CubeCoordinate cube;
    private OffsetCoordinate offset { get; set; }
    public CellState state;

    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;
    [SerializeField] private Color blockedColor;
    [SerializeField] private Color selectColor;

    public byte steps = 4;
    private SpriteRenderer spriteRenderer;

    public void SetOffset(int row, int column)
    {
        offset.row = row; 
        offset.column = column;
        cube = offset.ToCube();
    }

    private void OnMouseDown()
    {
        if (grid.drawState == DrawState.Block)
        {
            state = state == CellState.Blocked ? CellState.Inactive : CellState.Blocked;
            spriteRenderer.color = state == CellState.Blocked ? blockedColor : inactiveColor;
            return;
        }

        if (grid.drawState == DrawState.Reachable)       
        {
            
            List<Cell>[] cells = grid.Reachable(this,steps);
            for (int i = 0; i < cells.Length; i++)
            {               
                foreach (Cell cell in cells[i])
                {
                    cell.state = CellState.Active;
                    cell.spriteRenderer.color = activeColor;                  
                }
            }            
            return;
        }
    }

    private void OnMouseEnter()
    {
        if (state != CellState.Blocked)
        {
            spriteRenderer.color = selectColor;
        }

        if (grid.drawState == DrawState.Clear && state != CellState.Blocked)
        {
            state = CellState.Inactive;
        }
    }

    private void OnMouseExit()
    {
        if (state != CellState.Blocked)
            spriteRenderer.color = state == CellState.Active ? activeColor : inactiveColor;
    }


    private void Awake()
    {
        cube = new CubeCoordinate();
        offset = new OffsetCoordinate();
        grid = transform.parent.GetComponent<Grid>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        state = CellState.Inactive;
    }

    public int Distance(CubeCoordinate cube)
    {
        return (Math.Abs(this.cube.X - cube.X) + Math.Abs(this.cube.Y - cube.Y) + Math.Abs(this.cube.Z - cube.Z)) / 2;
    }

    public int Distance(OffsetCoordinate offset)
    {
        return Distance(offset.ToCube());
    }
    public int Distance(Cell cell)
    {
        return Distance(cell.cube);
    }

}
