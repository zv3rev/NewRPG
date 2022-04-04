using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


// ласс вертикальной (с плоским верхом) гексогональной сетки с  четным смещением (€чейки с четным столбцом смещены вниз)
/*
     —≈“ ј 3*4
     ___     ___
 ___/0,1\___/0,3\
/0,0\___/0,2\___/
\___/1,1\___/1,3\
/1,0\___/1,2\___/
\___/2,1\___/2,3\
/2,0\___/2,2\___/
\___/   \___/
 
*/

public class HexGrid: MonoBehaviour
{
    private int width;
    private int height;
    private HexCell[,] grid;

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


    // онструктор сетки высоты height и ширной width
    public void Initiate(int height, int width)
    {
        this.width = width;
        this.height = height;
        grid = new HexCell[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                grid[i, j] = new HexCell(i, j);
                grid[i, j].offset.row = i;
                grid[i, j].offset.column = j;
            }
        }
    }

    //¬озвращает ссылку на €чеку с координатами смещени€ offset
    public HexCell GetCell(OffsetCoordinate offset)
    {
        if (offset.column >= 0 && offset.column < width &&
            offset.row >= 0 && offset.row < height && offset != null)
            return grid[offset.row, offset.column];
        else
            return null;
    }

    /*¬озвращает соседнюю €чеку с номером neibourNum €чейки cell.
     *—оседи нумеруютс€ с верхней €чейки по часовой стрелке
                             ___     
                         ___/ 0 \___
                        / 5 \___/ 1 \
                        \___/ x \___/
                        / 4 \___/ 2 \
                        \___/ 3 \___/
                            \___/     */

    public HexCell Neighbor(HexCell cell, int neighbourNum)
    {
        OffsetCoordinate neighbourOffset = cell.cube.Neighbor(neighbourNum).ToOffset();
        if (neighbourOffset.row >= 0 && neighbourOffset.row < height &&
            neighbourOffset.column >= 0 && neighbourOffset.column < width)
            return GetCell(neighbourOffset);
        else return null;
    }

    public HexCell Neighbor(int row, int column, int neighbourNum)
    {
        return Neighbor(grid[row, column], neighbourNum);
    }

    //¬озвращает список всех соседей €чейки cell
    public List<HexCell> AllNeighbors (HexCell cell)
    {
        List<HexCell> result = new List<HexCell> ();
        for (int i = 0; i < 6; i++)
            if (Neighbor(cell, i) != null)
                result.Add(Neighbor(cell, i));
        return result;
    }

    //¬озвращает список €чеек образующих линию от €чейки point1 до point2
    public List<HexCell> Line(HexCell point1, HexCell point2)
    {
        int length = point1.Distance(point2);
        List<HexCell> cells = new List<HexCell>();
        for (int i = 0; i <= length; i++)
        {
            CubeCoordinate cube = CubeRound(Lerp(point1.cube.X, point2.cube.X, 1.0 / length * i),
                                            Lerp(point1.cube.Y, point2.cube.Y, 1.0 / length * i),
                                            Lerp(point1.cube.Z, point2.cube.Z, 1.0 / length * i));
            cells.Add(GetCell(cube.ToOffset()));
        }
        return cells;
    }

    //¬озвращает список €чеек равноудаленных от center на рассто€ние radius;
    public List<HexCell> Ring(HexCell center, int radius)
    {
        if (radius < 0)
            return null;
        List<HexCell> result = new List<HexCell>();
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

    //¬озвращает список €чеек удаленных от center не более чем на radius
    public List<HexCell> Circle(HexCell center, int radius)
    {
        List<HexCell> result = new List<HexCell>();
        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = Math.Max(-radius, -dx - radius); dy <= Math.Min(radius, -dx + radius); dy++)
            {
                result.Add(GetCell(center.cube.Add(new CubeCoordinate(dx, dy, -dx - dy)).ToOffset()));
            }
        }
        return result;
    }

    /**/
    /*public List<List<HexCell>> Reachable(HexCell cell, int length)
    {
        List<HexCell> visited = new List<HexCell>();
        visited.Add(cell); 
        List<List<HexCell>> result = new List<List<HexCell>>();
        result.Add(visited);

        for (int i = 1; i < length; i++)
        {
            result.Add(new List<HexCell>());
            foreach (HexCell hex in result[i-1])
            {

            }
        }

    }*/

    private void Start()
    {
        Vector2Int size = GetComponent<GridGenerator>().gridSize;
        Initiate(size.x, size.y);
    }
}