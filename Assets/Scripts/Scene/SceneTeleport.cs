using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    [SerializeField] private SceneName sceneNameToGo = SceneName.Scene1_Farm;
    [SerializeField] private Vector3 scenePositionToGo = new Vector3();

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out PlayerController player))
        {
            float xPosition = Mathf.Approximately(scenePositionToGo.x, 0) ? player.transform.position.x : scenePositionToGo.x;
            float yPosition = Mathf.Approximately(scenePositionToGo.y, 0) ? player.transform.position.y : scenePositionToGo.y;
            float zPosition = Mathf.Approximately(scenePositionToGo.z, 0) ? player.transform.position.z : scenePositionToGo.z;

            SceneControllerManager.Instance.FadeAndLoadScene(sceneNameToGo.ToString(), new Vector3(xPosition,yPosition,zPosition));

        }
    }
}
