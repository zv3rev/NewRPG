using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum DrawState
{
    Active,Clear,Block,Reachable
}

public class Grid : MonoBehaviour
{
    [SerializeField] public Vector2Int gridSize;
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private float cellOffset;
    private Cell[,] cells;

    public DrawState drawState = DrawState.Active;

    #region Private
    //Ћинейна€ интерпол€ци€ отрезка point1 - point 2
    private double Lerp(double point1, double point2, double length)
    {
        return point1 + (point2 - point1) * length;
    }

    //¬озвращает кубическую координату €чейки, к которой относитс€ точка (x,y,z)
    private CubeCoordinate CubeRound(double x, double y, double z)
    {
        int xRound = Convert.ToInt32(Math.Round(x));
        int yRound = Convert.ToInt32(Math.Round(y));
        int zRound = Convert.ToInt32(Math.Round(z));

        double xDiff = Math.Abs(xRound - x);
        double yDiff = Math.Abs(yRound - y);
        double zDiff = Math.Abs(zRound - z);

        if (xDiff > yDiff && xDiff > zDiff)
            xRound = -yRound - zRound;
        else if (yDiff > zDiff)
            yRound = -xRound - zRound;
        else
            zRound = -xRound - yRound;

        return new CubeCoordinate(xRound, yRound, zRound);
    }
    #endregion

    private void Start()
    {
        GenerateGrid();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            drawState = DrawState.Block;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            drawState = DrawState.Clear;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            drawState = DrawState.Active;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            drawState = DrawState.Reachable;
        }
    }

    /*private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit);
            Cell cell = hit.transform.gameObject.GetComponent<Cell>();
            Debug.Log(cell);    
            if (cell != null)
            {
                cell.GetComponent<SpriteRenderer>().color = selectColor;
            }
        }
        

        if (Input.GetKeyDown(KeyCode.C))
        {
            foreach (Cell cell in cells)
            {
                cell.state = cell.state == CellState.Blocked ? CellState.Blocked : CellState.Inactive;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(Physics.Raycast(ray, out hit))
            {
                Cell cell = hit.transform.gameObject.GetComponent<Cell>();
                if (cell != null)
                {
                    cell.state = cell.state == CellState.Blocked ? CellState.Blocked : CellState.Active;
                }
            }


        }

        if (Input.anyKeyDown)
        {
            Refresh();
        }
    }*/

    /*private void Refresh()
    {
        foreach(Cell cell in cells)
        {
            SpriteRenderer sr = cell.GetComponent<SpriteRenderer>();
            switch (cell.state)
            {
                case CellState.Active:
                    sr.color = activeColor;
                    break;
                case CellState.Inactive:
                    sr.color = inactiveColor;
                    break;
                case CellState.Blocked:
                    sr.color = blockedColor;
                    break;
            }
        }
    }*/

    private void GenerateGrid()
    {
        cells = new Cell[gridSize.y, gridSize.x];        

        float outerRadius = cellPrefab.GetComponent<SpriteRenderer>().bounds.size.x / 2.0f;
        float innerRadius = cellPrefab.GetComponent<SpriteRenderer>().bounds.size.y / 2.0f;

        for (int row = 0; row < gridSize.y; row++)
        {
            for (int col = 0; col < gridSize.x; col++)
            {
                Vector2 position = new Vector2();
                position.x = (3 * outerRadius / 2 + cellOffset / Mathf.Cos(Mathf.Deg2Rad * 30)) * col;
                position.y = (-2 * innerRadius - cellOffset) * row + (innerRadius + cellOffset / 2 * Mathf.Sin(Mathf.Deg2Rad * 30)) * (col % 2);
                Cell cell = Instantiate(cellPrefab, position, Quaternion.identity, transform);                
                cell.SetOffset(row, col);                
                cells[row, col] = cell;
                cell.name = $"{row},{col}";
            }
        }
    }

    public Cell GetCell(OffsetCoordinate offset)
    {
        if (offset.column >= 0 && offset.column < gridSize.x &&
            offset.row >= 0 && offset.row < gridSize.y && offset != null)
            return cells[offset.row, offset.column];
        else
            return null;
    }

    public Cell Neighbor(Cell cell, int neighbourNum)
    {
        OffsetCoordinate neighbourOffset = cell.cube.Neighbor(neighbourNum).ToOffset();
        return GetCell(neighbourOffset);
    }

    public Cell Neighbor(int row, int column, int neighbourNum)
    {
        return Neighbor(cells[row, column], neighbourNum);
    }

    public List<Cell> AllNeighbors(Cell cell)
    {
        List<Cell> result = new List<Cell>();
        for (int i = 0; i < 6; i++)
            if (Neighbor(cell, i) != null)
                result.Add(Neighbor(cell, i));
        return result;
    }

    public List<Cell> Line(Cell point1, Cell point2)
    {
        int length = point1.Distance(point2);
        List<Cell> result = new List<Cell>();
        for (int i = 0; i <= length; i++)
        {
            CubeCoordinate cube = CubeRound(Lerp(point1.cube.X, point2.cube.X, 1.0 / length * i),
                                            Lerp(point1.cube.Y, point2.cube.Y, 1.0 / length * i),
                                            Lerp(point1.cube.Z, point2.cube.Z, 1.0 / length * i));
            result.Add(GetCell(cube.ToOffset()));
        }
        return result;
    }

    public List<Cell> Ring(Cell center, int radius)
    {
        if (radius < 0)
            return null;
        List<Cell> result = new List<Cell>();
        if (radius == 0)
        {
            result.Add(center);
            return result;
        }

        CubeCoordinate cube = center.cube.Add(new CubeCoordinate(-radius, 0, radius));

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                if (GetCell(cube.ToOffset()) != null)
                    result.Add(GetCell(cube.ToOffset()));
                cube = cube.Neighbor(i);
            }
        }
        return result;
    }

    public List<Cell> Circle(Cell center, int radius)
    {
        List<Cell> result = new List<Cell>();
        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = Math.Max(-radius, -dx - radius); dy <= Math.Min(radius, -dx + radius); dy++)
            {
                Cell cell = GetCell(center.cube.Add(new CubeCoordinate(dx, dy, -dx - dy)).ToOffset());
                if (cell != null)
                    result.Add(cell); 
            }
        }
        return result;
    }

    public List<Cell> Sector(Cell center, int range, int direction)
    {
        direction = direction % 6;
        List<Cell> result = new List<Cell>();
        int leftX = 0, rightX = 0;
        int leftY = 0, rightY = 0;
        int leftZ = 0, rightZ = 0;

        switch (direction)
        {
            case 0:
                leftX = 0;
                rightX = range;
                leftY = 0;
                rightY = range;
                leftZ = -range;
                rightZ = 0;
                break;
            case 1:
                leftX = 0;
                rightX = range;
                leftY = -range;
                rightY = 0;
                leftZ = -range;
                rightZ = 0;
                break;
            case 2:
                leftX = 0;
                rightX = range;
                leftY = -range;
                rightY = 0;
                leftZ = 0;
                rightZ = range;
                break;
            case 3:
                leftX = -range;
                rightX = 0;
                leftY = -range;
                rightY = 0;
                leftZ = 0;
                rightZ = range;
                break;
            case 4:
                leftX = -range;
                rightX = 0;
                leftY = 0;
                rightY = range;
                leftZ = 0;
                rightZ = range;
                break;
            case 5:
                leftX = -range;
                rightX = 0;
                leftY = 0;
                rightY = range;
                leftZ = -range;
                rightZ = 0;
                break;
        }

        for (int dx = leftX; dx <= rightX; dx++)
            for (int dy = leftY; dy <= rightY; dy++)
                for (int dz = leftZ; dz <= rightZ; dz++)
                    if (dx + dy + dz == 0 && GetCell(center.cube.Add(new CubeCoordinate(dx, dy, dz)).ToOffset()) != null)
                        result.Add(GetCell(center.cube.Add(new CubeCoordinate(dx, dy, dz)).ToOffset()));

        return result;
    }

    public List<Cell>[] Reachable(Cell start, int steps)
    {
        List<Cell>[] result = new List<Cell>[steps + 1];
        result[0] = new List<Cell> {start};
        for (int i = 1; i <= steps; i++)
        {
            result[i] = new List<Cell>();
            foreach (Cell cell in result[i-1])
            {
                foreach (Cell neighborCell in AllNeighbors(cell))
                {
                    if (neighborCell.state != CellState.Blocked)
                        result[i].Add(neighborCell);
                }
            }
        }
        return result;
    }
}
