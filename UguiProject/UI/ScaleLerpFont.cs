using UnityEngine;
using UnityEngine.UI;

public class ScaleLerpFont : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var font = GetComponent<Text>();
        font.fontSize = (int)(((float)(font.fontSize)) * DB_Manager.dbManager.ScreenScaleLerp.y);
    }
}
