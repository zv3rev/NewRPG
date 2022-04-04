using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] public Vector2Int gridSize;
    [SerializeField] private Cell prefab;
    [SerializeField] private float cellOffset;
    [SerializeField] private Transform parent;

    public HexGrid grid;

    [ContextMenu("Generate grid")]
    private void GenerateGrid()
    {
        int children = transform.childCount;

        for (int i = 0; i < children; i++)     
            Destroy(transform.GetChild(i).gameObject);

        grid = GetComponent<HexGrid>();

        float outerRadius = prefab.GetComponent<SpriteRenderer>().bounds.size.x / 2.0f;
        float innerRadius = prefab.GetComponent<SpriteRenderer>().bounds.size.y / 2.0f;

        for (int row = 0; row < gridSize.x; row++)
        {
            for (int col = 0; col < gridSize.y; col++)
            {
                Vector2 position = new Vector2();

                position.x = (3 * outerRadius / 2 + cellOffset/Mathf.Cos(Mathf.Deg2Rad*30)) * col;
                position.y = (-2 * innerRadius-cellOffset) * row + (innerRadius + cellOffset / 2*Mathf.Sin(Mathf.Deg2Rad * 30)) * (col % 2);
                var cell = Instantiate(prefab, position, Quaternion.identity, parent);
                cell.name = $"{row},{col}";
                cell.cell = grid.GetCell(new OffsetCoordinate(row,col));
            }
        }
    }

    private void Start()
    {
        GenerateGrid();
    }
}
