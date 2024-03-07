using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class TilemapGridProperties : MonoBehaviour
{
#if UNITY_EDITOR
    private Tilemap tilemap;

    [SerializeField] private SO_GridProperties gridProperties = null;
    [SerializeField] private GridBoolProperty gridBoolProperty = GridBoolProperty.diggable;

    private void OnEnable()
    {
        if (!Application.IsPlaying(gameObject))
        {
            tilemap = GetComponent<Tilemap>();

            if(gridProperties != null)
            {
                gridProperties.gridPropertyList.Clear();
            }
        }
    }

    private void OnDisable()
    {
        if (!Application.IsPlaying(gameObject))
        {
            UseGridProperties();

            if (gridProperties != null)
            {
                EditorUtility.SetDirty(gridProperties);
            }
        }
    }

    private void Update()
    {
        if (!Application.IsPlaying(gameObject))
        {

        }
    }

    private void UseGridProperties()
    {
        tilemap.CompressBounds();

        if (!Application.IsPlaying(gameObject))
        {
            if(gridProperties != null)
            {
                Vector3Int startCell = tilemap.cellBounds.min;
                Vector3Int endCell = tilemap.cellBounds.max;

                for (int x = startCell.x; x < endCell.x; x++)
                {
                    for (int y = startCell.y; y < endCell.y; y++)
                    {
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                            
                        if(tile != null)
                        {
                            gridProperties.gridPropertyList.Add(new GridPropetry(new GridCoordinate(x, y), gridBoolProperty, true));
                        }
                    }
                }
            }
        }
    }
#endif
}
