using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class OffsetCoordinate
{
    public int column;
    public int row;

    //Коструктор координаты смещения (0,0)
    public OffsetCoordinate()
    {
        column = 0;
        row = 0;
    }

    //Конструктор координаты смещения (row,column)
    public OffsetCoordinate(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

    //Преобразование координаты смещения к кубической
    public CubeCoordinate ToCube()
    {
        CubeCoordinate result = new CubeCoordinate();
        result.X = column;
        result.Z = row - (column + (column & 1)) / 2;
        result.Y = -result.X - result.Z;
        return result;
    }
}
