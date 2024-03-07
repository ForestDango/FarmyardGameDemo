using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_AnimationType", menuName = "Scriptable Objects/Animation Type")]

public class SO_AnimationType : ScriptableObject
{
    public AnimationClip animationClip;
    public AnimationName animationName; //ö�٣�������
    public CharacterPartAnimator characterPart; //��ɫ����
    public ParVariantColor parVariantColor; //ö�٣���ͬ�������ɫ
    public ParVariantType parVariantType; //ö�٣������õĲ�ͬ����
}
