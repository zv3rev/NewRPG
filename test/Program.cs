using System;

namespace test
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            HexGrid grid = new HexGrid(2, 6);
            GridCell cell = new GridCell();
            cell.row = 1;
            cell.column = 3;
            Console.WriteLine(grid.Neighbour(cell,1).value);
            Console.ReadLine();
        }
    }
}