using System.Collections;
using System.Collections.Generic;

//����� ������������ (� ������� ������) �������������� ����� �  ������ ��������� (������ � ������ �������� ������� ����)
/*
     ����� 3*4
     ___     ___
 ___/0,1\___/0,3\
/0,0\___/0,2\___/
\___/1,1\___/1,3\
/1,0\___/1,2\___/
\___/2,1\___/2,3\
/2,0\___/2,2\___/
\___/   \___/
 
*/

public class HexGrid
{
    private int width;
    private int height;
    public GridCell[,] grid;

    public HexGrid(int width, int height)
    {
        this.width = width;
        this.height = height;   
        grid = new GridCell[width, height];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                grid[i, j] = new GridCell();
                grid[i, j].row = i;
                grid[i, j].column = j;                
                grid[i, j].value = Convert.ToInt32(Console.ReadLine());
            }
        }
    }

/*���������� �������� ����� � ������� neibourNum ������ cell.
 *������ ���������� � ������� ������ �� ������� �������
                         ___     
                     ___/ 0 \___
                    / 5 \___/ 1 \
                    \___/ x \___/
                    / 4 \___/ 2 \
                    \___/ 3 \___/
                        \___/     */ 
    public GridCell Neighbour(GridCell cell,int neighbourNum)
    {
        int row = cell.row;
        int col = cell.column;
        neighbourNum = neighbourNum % 6;

        switch (neighbourNum)
        {
            case 0: //������
                if (row == 0)
                    return null;
                else 
                    return grid[row-1,col];      
                
            case 1: //������ ������
                if (col % 2 == 0)
                    if (col == width - 1) return null;
                    else return grid[row, col + 1];
                else
                    if (row == 0 || col == width - 1) return null;
                    else return grid[row - 1, col + 1];
                
            case 2: //������ �����
                if (col % 2 == 0)
                    if (row == height -1 || col == width - 1) return null;
                    else return grid[row, col + 1];
                else
                    if (col == width - 1) return null;
                    else return grid[row - 1, col + 1];

            case 3: //�����
                if (row == height - 1)
                    return null;
                else
                    return grid[row + 1, col];
            case 4: //����� �����
                if (col % 2 == 0)
                    if (row == height - 1 || col == 0) return null;
                    else return grid[row+1, col - 1];
                else
                    if (col == 0) return null;
                    else return grid[row, col -1];
            case 5: //����� ������
                if (col % 2 == 0)
                    if (col == 0) return null;
                    else return grid[row, col + 1];
                else
                    if (row == 0 || col == 0) return null;
                    else return grid[row - 1, col - 1];
            default: return null;
        }
    }
}
