/// <summary>
/// 可以直接序列化被调用的三维向量
/// </summary>
[System.Serializable]
public class Vector3Serializable
{
    public float x;
    public float y;
    public float z;

    public Vector3Serializable(float x,float y,float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3Serializable()
    {

    }
}
