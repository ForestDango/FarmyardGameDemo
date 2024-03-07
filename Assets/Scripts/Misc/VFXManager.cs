using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特效管理器，继承自单例模式，当创建新的粒子系统的时候要在这里登记给对象池中
/// </summary>
public class VFXManager : Singleton<VFXManager>
{
    private WaitForSeconds twoSeconds;
    [SerializeField] private GameObject reapingPrefab = null;
    [SerializeField] private GameObject decidulousLeavsFallPrefab = null;
    [SerializeField] private GameObject choppingTreeTrunkPrefab = null;
    [SerializeField] private GameObject pineConeLeavesFallPrefab = null;
    [SerializeField] private GameObject breakingStonePrefab = null;
    protected override void Awake()
    {
        base.Awake();

        twoSeconds = new WaitForSeconds(2f);
    }

    private void OnDisable()
    {
        EventHandler.HarvestActionEvent -= DisplayHarvestActionEvent;
    }

    private void OnEnable()
    {
        EventHandler.HarvestActionEvent += DisplayHarvestActionEvent;
    }

    private IEnumerator DisplayHarvestsActionEvnetCoroutine(GameObject effectGameObject,WaitForSeconds secondsToWait)
    {
        yield return secondsToWait;
        effectGameObject.SetActive(false);
    }

    private void DisplayHarvestActionEvent(Vector3  effectPosition, HarvestEffect harvestEffect)
    {
        switch (harvestEffect)
        {
            case HarvestEffect.reaping:
                GameObject reaping = PoolManager.Instance.ReuseObject(reapingPrefab, effectPosition, Quaternion.identity);
                reaping.SetActive(true);
                StartCoroutine(DisplayHarvestsActionEvnetCoroutine(reaping, twoSeconds));
                break;

            case HarvestEffect.decidousLeavesFalling:
                GameObject decidousLeavesFalling = PoolManager.Instance.ReuseObject(decidulousLeavsFallPrefab, effectPosition, Quaternion.identity);
                decidousLeavesFalling.SetActive(true);
                StartCoroutine(DisplayHarvestsActionEvnetCoroutine(decidousLeavesFalling, twoSeconds));
                break;

            case HarvestEffect.pineConesFalling:
                GameObject pineConesFalling = PoolManager.Instance.ReuseObject(pineConeLeavesFallPrefab, effectPosition, Quaternion.identity);
                pineConesFalling.SetActive(true);
                StartCoroutine(DisplayHarvestsActionEvnetCoroutine(pineConesFalling, twoSeconds));
                break;

            case HarvestEffect.breakingStone:
                GameObject breakingStone = PoolManager.Instance.ReuseObject(breakingStonePrefab, effectPosition, Quaternion.identity);
                breakingStone.SetActive(true);
                StartCoroutine(DisplayHarvestsActionEvnetCoroutine(breakingStone, twoSeconds));
                break;

            case HarvestEffect.choppingTreeTrunk:
                GameObject choppingTreeTrunk = PoolManager.Instance.ReuseObject(choppingTreeTrunkPrefab, effectPosition, Quaternion.identity);
                choppingTreeTrunk.SetActive(true);
                StartCoroutine(DisplayHarvestsActionEvnetCoroutine(choppingTreeTrunk, twoSeconds));
                break;

            case HarvestEffect.none:
                break;

            default:
                break;
        }
    }
}
