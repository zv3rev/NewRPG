using System;

namespace test
{
    internal class Program
    {

        static void Main(string[] args)
        {
            HexGrid grid = new HexGrid(3, 5);
            for (int i = 0; i < grid.height; i++)
            {
                for (int j = 0; j < grid.width; j++)
                {
                    grid.grid[i,j].value = Convert.ToInt32(Console.ReadLine());
                }
            }
            GridCell cell = grid.grid[1, 2];

            GridCell neighbour = grid.Neighbour(cell, 0);
            Console.Write("Сверху: ");
            if (neighbour != null)
                Console.WriteLine(neighbour.value);
            else
                Console.WriteLine("NULL");

            neighbour = grid.Neighbour(cell, 5);
            Console.Write("Слева сверху: ");
            if (neighbour != null)
                Console.WriteLine(neighbour.value);
            else
                Console.WriteLine("NULL");

            neighbour = grid.Neighbour(cell, 1);
            Console.Write("Справа сверху: ");
            if (neighbour != null)
                Console.WriteLine(neighbour.value);
            else
                Console.WriteLine("NULL");

            neighbour = grid.Neighbour(cell, 4);
            Console.Write("Слева снизу: ");
            if (neighbour != null)
                Console.WriteLine(neighbour.value);
            else
                Console.WriteLine("NULL");

            neighbour = grid.Neighbour(cell, 2);
            Console.Write("Справа снизу: ");
            if (neighbour != null)
                Console.WriteLine(neighbour.value);
            else
                Console.WriteLine("NULL");

            neighbour = grid.Neighbour(cell, 3);
            Console.Write("Снизу: ");
            if (neighbour != null)
                Console.WriteLine(neighbour.value);
            else
                Console.WriteLine("NULL");


            //Console.WriteLine("  "+grid.Neighbour(cell,0).value);
            //Console.WriteLine(grid.Neighbour(cell, 5).value + "   "+ grid.Neighbour(cell, 1).value);
            //Console.WriteLine("  " + cell.value);
            //Console.WriteLine(grid.Neighbour(cell, 4).value + "   " + grid.Neighbour(cell, 2).value);
            //Console.WriteLine("  " + grid.Neighbour(cell, 3).value);
            Console.ReadLine();
        }
    }
}