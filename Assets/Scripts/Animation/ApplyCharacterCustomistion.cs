using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorSwap
{
    public Color fromColor;
    public Color toColor;

    public ColorSwap(Color fromColor,Color toColor)
    {
        this.fromColor = fromColor;
        this.toColor = toColor;
    }
}

public class ApplyCharacterCustomistion : MonoBehaviour
{
    [Header("Base Texture")]
    [SerializeField] private Texture2D maleFarmerTexture;
    [SerializeField] private Texture2D femaleFarmerTexture;
    [SerializeField] private Texture2D shirtsBaseTexture;
    private Texture2D farmerBaseTexure;


    [Header("OutputBase Texture To Be Used For Animation")]
    [SerializeField] private Texture2D farmerBasedCustomised = null;
    //private Texture2D farmerBasedShirt;
    //private Texture2D selectedShirt;

    //[Header("Select Shirt Style")]
    //[Range(0, 1)]
    //[SerializeField] private int inputShirtStyleNo = 0;

    [Header("Select Sex ,1-male,0-female")]
    [Range(0, 1)]
    [SerializeField] private int inputSex = 0;

    //private Facing[,] bodyFacingArray;
    //private Vector2Int[,] bodyShirtOffsetArray;

    //private int bodyRows = 21;

    //private int bodyColoums = 6;
    //private int farmerSpriteWidth = 16;
    //private int farmerSpriteHeight = 32;

    //private int shirtTexureWidth = 9;
    //private int shirtTextureHeight = 36;
    //private int shirtSpriteWidth = 9;
    //private int shirtSpriteHeight = 9;
    //private int shirtStylesInSpriteWidth = 16;

    private List<ColorSwap> colorSwapList;

    //private Color32 armTargetColor1 = new Color32(77, 13, 13, 255);
    //private Color32 armTargetColor2 = new Color32(138, 41, 41, 255);
    //private Color32 armTargetColor3 = new Color32(172, 50, 50, 255);


    private void Awake()
    {
        colorSwapList = new List<ColorSwap>();

        ProcessCustomisation();
    }

    private void ProcessCustomisation()
    {
        ProcessGender();

        ProcessShirt();

        ProcessArms();

        MergeCustomisationn();
    }

    private void MergeCustomisationn()
    {
        
    }

    private void ProcessArms()
    {
        
    }

    private void ProcessShirt()
    {
            
    }

    private void ProcessGender()
    {
        if(inputSex == 0)
        {
            farmerBaseTexure = maleFarmerTexture;
        }
        else if(inputSex == 1)
        {
            farmerBaseTexure = femaleFarmerTexture;
        }

        Color[] farmerBasePixel = farmerBaseTexure.GetPixels();

        farmerBasedCustomised.SetPixels(farmerBasePixel);
        farmerBasedCustomised.Apply();
    }
}
