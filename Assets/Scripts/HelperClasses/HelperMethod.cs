using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperMethod 
{
    /// <summary>
    /// 泛型，在指针脚本中被调用，当指针类型为ReapingTool的时候，判断前方对应OverlapPointAll的数量
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="componenetAtPositionList"></param>
    /// <param name="positionToCheck"></param>
    /// <returns></returns>
    public static bool GetComponenetAtCursorLocation<T>(out List<T> componenetAtPositionList,Vector3 positionToCheck)
    {
        bool found = false;

        List<T> componentList = new List<T>();

        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(positionToCheck);

        T tComponent = default(T);

        for (int i = 0; i < collider2DArray.Length; i++)
        {
            tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
            if(tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }
            else
            {
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();
                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }

        componenetAtPositionList = componentList;
        return found;
    }

    /// <summary>
    /// 泛型，在指针脚本中被调用，当指针类型为ReapingTool的时候，判断前方对应OverlapBoxNonAlloc的数量,可以减少内部性能消耗
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="numberOfCollisionToSet"></param>
    /// <param name="point"></param>
    /// <param name="size"></param>
    /// <param name="angle"></param>
    /// <returns></returns>

    public static T[] GetComponentAtBoxLocationNonAlloc<T>(int numberOfCollisionToSet,Vector2 point,Vector2 size,float angle)
    {
        Collider2D[] collider2DArray = new Collider2D[numberOfCollisionToSet];

        Physics2D.OverlapBoxNonAlloc(point, size, angle, collider2DArray);

        T tComponent = default(T);

        T[] componentArray = new T[collider2DArray.Length];

        for (int i = collider2DArray.Length -1; i >= 0; i--)
        {
            if(collider2DArray[i] != null)
            {
                tComponent = collider2DArray[i].gameObject.GetComponent<T>();

                if(tComponent!= null)
                {
                    componentArray[i] = tComponent;
                }
            }
        }

        return componentArray;
    }
    /// <summary>
    /// 泛型，在指针脚本中被调用，当指针类型为HoeingTool的时候，判断前方对应OverlapBoxAll的数量
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="listComponentsAtBoxLocation"></param>
    /// <param name="point"></param>
    /// <param name="size"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static bool GetComponenetsAtBoxLocation<T>(out List<T> listComponentsAtBoxLocation,Vector2 point,Vector2 size,float angle) 
    {
        bool found = false;
        List<T> compoenentT = new List<T>();

        Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(point, size, angle);

        for (int i = 0; i < collider2DArray.Length; i++)
        {
            T tComponenet = collider2DArray[i].gameObject.GetComponentInParent<T>();
            if(tComponenet != null)
            {
                found = true;
                compoenentT.Add(tComponenet);
            }
            else
            {
                tComponenet = collider2DArray[i].gameObject.GetComponentInChildren<T>();
                if(tComponenet != null)
                {
                    found = true;
                    compoenentT.Add(tComponenet);
                }
            }
        }

        listComponentsAtBoxLocation = compoenentT;

        return found;   
    }
}
