using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_AnimationType", menuName = "Scriptable Objects/Animation Type")]

public class SO_AnimationType : ScriptableObject
{
    public AnimationClip animationClip;
    public AnimationName animationName; //枚举，动画名
    public CharacterPartAnimator characterPart; //角色部分
    public ParVariantColor parVariantColor; //枚举，不同种类的颜色
    public ParVariantType parVariantType; //枚举，手上拿的不同种类
}
