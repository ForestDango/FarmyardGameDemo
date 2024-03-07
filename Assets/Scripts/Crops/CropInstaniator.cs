using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class CropInstaniator : MonoBehaviour
{
    private Grid grid;
    [SerializeField] private int daysSinceDug = -1;
    [SerializeField] private int daysSinceWatered = -1;
    [ItemCodeDescription]
    [SerializeField] private int seedItemCode = 0;
    [SerializeField] private int growthDays = 0;


    private void OnEnable()
    {
        EventHandler.InstantiateCropPrefabEvent += InstantiateCropPrefab;
    }

    private void OnDisable()
    {
        EventHandler.InstantiateCropPrefabEvent -= InstantiateCropPrefab;
    }

    private void InstantiateCropPrefab()
    {
        grid = FindObjectOfType<Grid>();

        Vector3Int cropGridPosition = grid.WorldToCell(transform.position);

        SetCropProperties(cropGridPosition);

        Destroy(gameObject);
    }

    private void SetCropProperties(Vector3Int cropGridPosition)
    {
        if(seedItemCode > 0)
        {
            GridPropertyDetails gridPropertyDetails;

            gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropetyDetails(cropGridPosition.x, cropGridPosition.y);

            if(gridPropertyDetails != null)
            {
                gridPropertyDetails = new GridPropertyDetails();
            }

            gridPropertyDetails.daysSinceDug = daysSinceDug;
            gridPropertyDetails.daysSinceWatered = daysSinceWatered;
            gridPropertyDetails.seedItemCode = seedItemCode;
            gridPropertyDetails.growthDays = growthDays;

            GridPropertiesManager.Instance.SetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y, gridPropertyDetails);
        }
    }
}
