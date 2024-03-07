using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EggsDetailsList", menuName = "Scriptable Objects/Eggs/Egg Details List")]
public class SO_EggDetailsList : ScriptableObject
{
    [SerializeField] public List<EggDetails> eggDetailsList;

    public EggDetails GetEggDetails(int eggItemCode)
    {
        return eggDetailsList.Find(x => x.eggItemCode == eggItemCode);
    }
}
