using System.Collections;
using System.Collections.Generic;
using System;

public class HexCell
{
    public CubeCoordinate cube;
    public OffsetCoordinate offset;
    public char value;
    public bool isBlocked;

    //����������� ������ � ����������� �������� (row,column)
    public HexCell(int row, int column)
    {
        value = '.';
        offset = new OffsetCoordinate();
        offset.row = row;
        offset.column = column;
        cube = new CubeCoordinate();
        cube = offset.ToCube();
    }

    //���������� �� ������ �� ������ � ���������� ������������ cube
    public int Distance(CubeCoordinate cube)
    {
        return (Math.Abs(this.cube.X - cube.X) + Math.Abs(this.cube.Y - cube.Y) + Math.Abs(this.cube.Z - cube.Z)) / 2;
    }

    //���������� �� ������ �� ������ � ����������� �������� offset
    public int Distance(OffsetCoordinate offset)
    {
        return Distance(offset.ToCube());
    }

    //���������� �� ������ cell
    public int Distance(HexCell cell)
    {
        return Distance(cell.cube);
    }
}
