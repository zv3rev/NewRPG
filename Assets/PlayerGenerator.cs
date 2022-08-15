using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGenerator : MonoBehaviour
{
    public Player playerPrefab;

   public List<Player> GeneratePlayers(Grid grid, int playersAmount)
    {
        List<Player> result = new List<Player>();
        for (int i = 0; i < playersAmount; i++)
        {
            Cell cell;
            do
            {
                cell = grid.GetCell(new OffsetCoordinate(new System.Random().Next(0, grid.gridSize.y), new System.Random().Next(0, grid.gridSize.x)));
            } while (cell.isOcupied || cell.StepCost == 0);
            Player player = Instantiate(playerPrefab, cell.transform.position, Quaternion.identity, transform);
            player.grid = grid;
            player.currentPlace = cell;
            cell.isOcupied = true;
            result.Add(player);
        }
        return result;
    }
}
