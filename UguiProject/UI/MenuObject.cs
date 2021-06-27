using UnityEngine;
using UnityEngine.UI;

public class MenuObject : MonoBehaviour
{
    void Start()
    {
        (transform as RectTransform).offsetMax = new Vector2(0f, (transform as RectTransform).offsetMax.y*DB_Manager.dbManager.ScreenScaleLerp.y);
        var content = transform.GetChild(0).gameObject;
        var grid = content.GetComponent<GridLayoutGroup>();
        var size = grid.cellSize;
        size.x *= DB_Manager.dbManager.ScreenScaleLerp.x;
        size.y *= DB_Manager.dbManager.ScreenScaleLerp.y;
        grid.cellSize = size;

        var spacing = grid.spacing;
        spacing.x *= DB_Manager.dbManager.ScreenScaleLerp.x;
        spacing.y *= DB_Manager.dbManager.ScreenScaleLerp.y;
        grid.spacing = spacing;

        var contentRT = content.transform as RectTransform;
        var offsetMin = contentRT.offsetMin;
        offsetMin.x *= DB_Manager.dbManager.ScreenScaleLerp.x;
        var offsetMax = contentRT.offsetMax;
        offsetMax.x = -offsetMin.x;
        contentRT.offsetMin = offsetMin;
        contentRT.offsetMax = offsetMax;
    }
}
