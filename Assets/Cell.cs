using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*public enum CellState
{
    Active, Inactive, Blocked
}*/

public class Cell : MonoBehaviour
{
    private Grid grid;
    public CubeCoordinate cube;
    private OffsetCoordinate offset { get; set; }
    public Cell previous = null;
    public bool isOcupied = false;
    [SerializeField] private Information information; 

    [SerializeField] private float _shootChance = 1;    
    public float ShootChance
    {
        get => _shootChance;
        set
        {
            if (value < 0)
            {
                _shootChance = 0;
                return;
            }
            if (value>1)
            {
                _shootChance = 1;
                return;
            }
            _shootChance = value;

        }
    }

    [SerializeField] private byte _stepCost = 1;
    public byte StepCost
    {
        get => _stepCost;
        set => _stepCost = value;
    }

    [SerializeField] private Color reachableColor;
    [SerializeField] private Color blockedColor;
    [SerializeField] private Color notReachableColor;

    private SpriteRenderer spriteRenderer;

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }

    public void SetBaseColor()
    {
        spriteRenderer.color = StepCost > 0 ? new Color(1f - 0.2f * StepCost, 1f - 0.2f * StepCost, 1f - 0.2f * StepCost) : blockedColor;
    }

    public void SetOffset(int row, int column)
    {
        offset.row = row; 
        offset.column = column;
        cube = offset.ToCube();
    }

    private void OnMouseDown()
    {
        
    }

    private void OnMouseEnter()
    {
        information.offsetCoordinates = $"{offset.row},{offset.column}";
        information.cost = StepCost;

        Player player = grid.GetCurrentPlayer();
        if (player != null && !isOcupied)
        {
            List<Cell> path = grid.FindPath(player.currentPlace, this);
            if (path != null && !isOcupied)
            {
                path.Reverse();
                path.RemoveAt(0);
                int stepsAmount = player.stepsAmount;
                foreach (Cell cell in path)
                {
                    stepsAmount -= cell.StepCost;
                    cell.SetColor(stepsAmount >= 0 ? reachableColor : notReachableColor);
                }
                information.actionPoints = grid.GetCurrentPlayer().stepsAmount - stepsAmount;
            }
            else
            {
                information.actionPoints = 0;
            }
        }    

        information.UpdateInformation();
    }

    private void OnMouseExit()
    {
        information.ClearInformattion();
        grid.ClearGrid();
    }

    private void Awake()
    {
        cube = new CubeCoordinate();
        offset = new OffsetCoordinate();
        grid = transform.parent.GetComponent<Grid>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        information = GameObject.Find("Information").GetComponent<Information>();
        StepCost = (byte)new System.Random().Next(0, 5);
        SetBaseColor();
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
