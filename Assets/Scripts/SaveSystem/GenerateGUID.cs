
using UnityEngine;

[ExecuteAlways]
public class GenerateGUID : MonoBehaviour
{
    [SerializeField] private string _gUID = "";

    public string GUID
    {
        get => _gUID;
        set => _gUID = value;
    }

    private void Awake()
    {
        if (!Application.IsPlaying(gameObject))
        {
            if(_gUID == "")
            {

                //�Ǽ�GUID
                _gUID = System.Guid.NewGuid().ToString();
            }
        }
    }
}
