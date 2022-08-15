using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum DrawState
{   
    Block,
    CharacterMove,
    Play,
    ChangeCost
}

public class Grid : MonoBehaviour
{
    [SerializeField] public Vector2Int gridSize;
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private float cellOffset;
    private Cell[,] cells;

    [SerializeField] private const int playersAmount = 3;
    [SerializeField] private PlayerGenerator playerGenerator;
    private Player[] players = new Player[playersAmount];

    [SerializeField] private const int enemiesAmount = 3;
    [SerializeField] private PlayerGenerator enemyGenerator;
    private Player[] enemies = new Player[enemiesAmount];

    private bool isEnemyTurn = false;
    private Player currentCharacter = null;

    public DrawState drawState = DrawState.Block;

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

    private Cell ChooseNearestNode(List<Cell> cells, Dictionary<Cell, int> costs, Cell goal)
    {
        Cell bestNode = cells[0];
        int minCost = costs[bestNode] + goal.Distance(bestNode);
        foreach (Cell cell in cells)
        {
            int totalCost = costs[cell] + goal.Distance(cell) * 2;
            if (totalCost < minCost)
            {
                minCost = totalCost;
                bestNode = cell;
            }
        }
        return bestNode;
    }

    private List<Cell> BuildPath(Cell goal)
    {
        List<Cell> path = new List<Cell>();
        while (goal != null)
        {
            path.Add(goal);
            goal = goal.previous;
        }
        return path;
    }
    

    private void Start()
    {
        GenerateGrid();
        players = playerGenerator.GeneratePlayers(this, playersAmount).ToArray();
        enemies = enemyGenerator.GeneratePlayers(this, enemiesAmount).ToArray();
    }

    #endregion

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

    public void InactiveCells()
    {
        foreach (Cell cell in cells)
        {
            cell.SetColor(new Color(51 * cell.StepCost, 51 * cell.StepCost, 51 * cell.StepCost));

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
                string[] a = new string[5];
                foreach (Cell neighborCell in AllNeighbors(cell))
                {
                    if (neighborCell.StepCost != 0)
                        result[i].Add(neighborCell);
                }
            }
        }
        return result;
    }

    public List<Cell> FindPath(Cell start, Cell end)
    {
        List<Cell> reachable = new List<Cell>();
        reachable.Add(start);
        List<Cell> explored = new List<Cell>();
        Dictionary<Cell, int> costs = new Dictionary<Cell, int>();
        costs.Add(start, 0);

        
        while (reachable.Count != 0)
        {
            Cell cell = ChooseNearestNode(reachable, costs, end);

            if (cell == end)
            {
                return BuildPath(end);
            }

            reachable.Remove(cell);
            explored.Add(cell);
            List<Cell> cellNeighbors = AllNeighbors(cell);
            foreach (Cell neighbor in cellNeighbors)
            {
                if (neighbor.StepCost!=0 && !reachable.Contains(neighbor) && !explored.Contains(neighbor))
                {
                    reachable.Add(neighbor);
                }

                if (!costs.ContainsKey(neighbor) || costs[cell] + neighbor.StepCost < costs[neighbor])
                {
                    neighbor.previous = cell;
                    costs.Remove(neighbor);
                    costs.Add(neighbor, costs[cell] + neighbor.StepCost);
                }
            }
        }
        return null;
    } 

    public Player GetCurrentPlayer()
    {
        return currentCharacter;
    }

    public void ClearGrid()
    {
        foreach (Cell cell in cells)
        {
            cell.SetBaseColor() ;
        }
    }

    public void SetCurrentPlayer(Player character)
    {
        if (currentCharacter != null)
        {
            currentCharacter.Unselect();
        }        
        currentCharacter = character;
        currentCharacter.Select();
    }
}
