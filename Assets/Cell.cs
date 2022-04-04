using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private Color standartColor;
    [SerializeField] private Color hoverColor;
    private SpriteRenderer spriteRenderer;
    private HexGrid grid;
    public HexCell cell;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        grid = transform.parent.GetComponent<HexGrid>();
    }

    private void Update()
    {
        spriteRenderer.color = cell.color;        
    }

    private void OnMouseEnter()
    {
        cell.color = hoverColor;
    }

    private void OnMouseExit()
    {
        cell.color = standartColor;
    }

    private void OnMouseDown()
    {
        grid = transform.parent.GetComponent<HexGrid>();
        List<HexCell> cells = grid.Ring(cell,3);
        foreach (HexCell tmp in cells)
        {
            tmp.color = hoverColor;
        }
    }
}
