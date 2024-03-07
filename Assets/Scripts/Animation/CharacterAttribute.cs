

[System.Serializable]
public struct CharacterAttribute 
{
    public CharacterPartAnimator characterPart;
    public ParVariantColor parVariantColor;
    public ParVariantType parVariantType;

    public CharacterAttribute(CharacterPartAnimator characterPart, ParVariantColor parVariantColor, ParVariantType parVariantType)
    {
        this.characterPart = characterPart;
        this.parVariantColor = parVariantColor;
        this.parVariantType = parVariantType;
    }
}
