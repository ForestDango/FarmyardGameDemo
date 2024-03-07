using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{
    [SerializeField] private GameObject character = null;
    [SerializeField] private SO_AnimationType[] soAnimationTypeArray = null;

    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionaryByAnimation; //key = AnimationClip,value = SO_AnimationType
    private Dictionary<string, SO_AnimationType> animationTypeDictionaryByCompositeAttibuteKey; //key = CompositeAttibuteKey value = SO_AnimationType

    private void Start()
    {
        //先初始化字典by animatonClip

        animationTypeDictionaryByAnimation = new Dictionary<AnimationClip, SO_AnimationType>();

        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            animationTypeDictionaryByAnimation.Add(item.animationClip, item);
        }

        //先初始化字典by string
        animationTypeDictionaryByCompositeAttibuteKey = new Dictionary<string, SO_AnimationType>();

        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            string key = item.characterPart.ToString() + item.parVariantColor.ToString() + item.parVariantType.ToString() + item.animationName.ToString();
            animationTypeDictionaryByCompositeAttibuteKey.Add(key, item);
        }
    }


    public void ApplyCharacterCustomsationParameters(List<CharacterAttribute> characterAttributes)
    {
        //遍历CharacterAttribute并且把符文条件的设置animator controller override
        foreach (CharacterAttribute  characterAttribute in characterAttributes)
        {
            Animator currentAnimator = null;
            List<KeyValuePair<AnimationClip, AnimationClip>> animsKeyValuePairList = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            string animatorSOAssetName = characterAttribute.characterPart.ToString();

            Animator[] animatorArray = character.GetComponentsInChildren<Animator>();

            foreach (Animator animator  in animatorArray)
            {
                if(animator.name == animatorSOAssetName)
                {
                    currentAnimator = animator; //找到我们要覆盖的Animator
                    break;
                }
            }

            //然后得到当前动画器中的基类动画
            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            List<AnimationClip> animationList = new List<AnimationClip>(aoc.animationClips);

            foreach (AnimationClip animationClip  in animationList)
            {
                SO_AnimationType so_AnimationType;
                bool foundAnimation = animationTypeDictionaryByAnimation.TryGetValue(animationClip, out so_AnimationType);

                if (foundAnimation)
                {
                    string key = characterAttribute.characterPart.ToString() + characterAttribute.parVariantColor.ToString() +
                        characterAttribute.parVariantType.ToString() + so_AnimationType.animationName.ToString();

                    SO_AnimationType swapSo_AnimationType;
                    bool foundSwapAnimation = animationTypeDictionaryByCompositeAttibuteKey.TryGetValue(key, out swapSo_AnimationType);

                    if (foundSwapAnimation)
                    {
                        AnimationClip swapAnimationClip = swapSo_AnimationType.animationClip;

                        animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));
                    }
                }
            }
            //把新找到的animsKeyValuePairList更新
            aoc.ApplyOverrides(animsKeyValuePairList);
            currentAnimator.runtimeAnimatorController = aoc;
         }


    }
}
