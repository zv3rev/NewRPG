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

    public byte steps = 8;
    private SpriteRenderer spriteRenderer;

    public void SetState(CellState newState)
    {
        

        switch (newState)
        {
            case CellState.Active:
                state = CellState.Active;
                spriteRenderer.color = activeColor;
                break;
            case CellState.Inactive:
                state = CellState.Inactive;
                spriteRenderer.color = inactiveColor;
                break;
            case CellState.Blocked:
                state = CellState.Blocked;
                spriteRenderer.color = blockedColor;
                break;            
            default:
                break;
        }

    }

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
            grid.InactiveCells();
            grid.player.DrawVariants();
            return;
        }

        if (grid.drawState == DrawState.Reachable && state != CellState.Blocked)       
        {  
            List<Cell>[] cells = grid.Reachable(this,steps);
            for (int i = 0; i < cells.Length; i++)
            {               
                foreach (Cell cell in cells[i])
                {
                    cell.SetState(CellState.Active);            
                }
            }            
            return;
        }

        if (grid.drawState == DrawState.CharacterMove && state != CellState.Blocked)
        {
            grid.player.currentPlace = this;
            grid.player.MoveToCell();
            return;
        }

        if (grid.drawState == DrawState.Play && state == CellState.Active)
        {
            grid.player.currentPlace = this;
            grid.player.MoveToCell();
            grid.InactiveCells();
            grid.player.DrawVariants();
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
