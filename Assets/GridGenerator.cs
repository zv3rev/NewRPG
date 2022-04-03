using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Cell prefab;
    [SerializeField] private float cellOffset;
    [SerializeField] private Transform parent;

    [ContextMenu("Generate grid")]
    private void GenerateGrid()
    {
        float outerRadius = prefab.GetComponent<SpriteRenderer>().bounds.size.x / 2.0f;
        float innerRadius = prefab.GetComponent<SpriteRenderer>().bounds.size.y / 2.0f;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2 position = new Vector2();

                //position.y = (x + y * 0.5f - y / 2) * (innerRadius * 2f);
                //position.x = y * (outerRadius * 1.5f);

                position.x = (3 * outerRadius / 2 + cellOffset/Mathf.Cos(Mathf.Deg2Rad*30)) * y;
                position.y = (-2 * innerRadius-cellOffset) * x + (innerRadius + cellOffset / 2*Mathf.Sin(Mathf.Deg2Rad * 30)) * (y % 2);
                var cell = Instantiate(prefab, position, Quaternion.identity, parent);
                cell.name = $"{x},{y}";
            }
        }
    }
}
