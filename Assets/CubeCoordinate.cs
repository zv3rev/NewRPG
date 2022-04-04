using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CubeCoordinate
{
    public int X;
    public int Y;
    public int Z;

    //Конструктор кубической координаты (X,Y,Z)
    public CubeCoordinate(int x, int y, int z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }
    
    //Конструктор кубической координаты (0,0,0)
    public CubeCoordinate()
    {
        this.X = 0;
        this.Y = 0;
        this.Z = 0;
    }

    //Перевод кубической координаты в координату смещения
    public OffsetCoordinate ToOffset()
    {
        OffsetCoordinate result = new OffsetCoordinate();
        result.column = X;
        result.row = Z + (X + (X & 1)) / 2;
        return result;
    }

    /*Возвращает кубическую координату соседней ячеки с номером neibourNum
     *Соседи нумеруются с верхней ячейки по часовой стрелке
                         ___     
                     ___/ 0 \___
                    / 5 \___/ 1 \
                    \___/ x \___/
                    / 4 \___/ 2 \
                    \___/ 3 \___/
                        \___/     */
    public CubeCoordinate Neighbor(int neighbourNum)
    {
        CubeCoordinate result = new CubeCoordinate();
        neighbourNum = neighbourNum % 6;
        switch (neighbourNum)
        {
            case 0:
                result.X = this.X;
                result.Y = this.Y + 1;
                result.Z = this.Z - 1;
                return result;
            case 1:
                result.X = this.X + 1;
                result.Y = this.Y;
                result.Z = this.Z - 1;
                return result;
            case 2:
                result.X = this.X + 1;
                result.Y = this.Y - 1;
                result.Z = this.Z;
                return result;
            case 3:
                result.X = this.X;
                result.Y = this.Y - 1;
                result.Z = this.Z + 1;
                return result;
            case 4:
                result.X = this.X - 1;
                result.Y = this.Y;
                result.Z = this.Z + 1;
                return result;
            case 5:
                result.X = this.X - 1;
                result.Y = this.Y + 1;
                result.Z = this.Z;
                return result;
            default: return null;
        }
    }

    //Сложение координат this и cube
    public CubeCoordinate Add(CubeCoordinate cube)
    {
        return new CubeCoordinate(X + cube.X, Y + cube.Y, Z + cube.Z);
    }
}
