using UnityEngine;
using UnityEngine.UI;

public class ScaleLerpContect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var grid = GetComponent<GridLayoutGroup>();

        var size = grid.cellSize;
        size.x *= DB_Manager.dbManager.ScreenScaleLerp.x;
        size.y *= DB_Manager.dbManager.ScreenScaleLerp.y;
        grid.cellSize = size;

        var spacing = grid.spacing;
        spacing.x *= DB_Manager.dbManager.ScreenScaleLerp.x;
        spacing.y *= DB_Manager.dbManager.ScreenScaleLerp.y;
        grid.spacing = spacing;
    }
}
