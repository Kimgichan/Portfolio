using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SprayObjectList : SerializableDictionary<string, SprayObject> { }

[System.Serializable]
public class SprayObjectDB : SerializableDictionary<string, SprayObjectList> { }

public class DB_Manager : MonoBehaviour
{
    public static DB_Manager dbManager;
    [SerializeField] Vector2 screenScaleLerp;
    [HideInInspector] public Vector2 ScreenScaleLerp
    {
        get
        {
            return screenScaleLerp;
        }
    }

    [SerializeField] SprayObjectDB sprayObjectDB;
    [SerializeField] SprayImageObject imagePrefab;
    [SerializeField] int imageObjectCount;
    [SerializeField] SprayTextObject textPrefab;
    [SerializeField] int textObjectCount;
    Stack<SprayImageObject> imageStack;
    Stack<SprayTextObject> textStack;



    void Awake()
    {
        dbManager = this;
        screenScaleLerp.x = Screen.width / screenScaleLerp.x;
        screenScaleLerp.y = Screen.height / screenScaleLerp.y;
        imageStack = new Stack<SprayImageObject>();
        textStack = new Stack<SprayTextObject>();
        for (int i = 0; i < imageObjectCount; i++)
        {
            imageStack.Push(Instantiate(imagePrefab));
            
        }
        for(int i = 0; i < textObjectCount; i++)
        {
            textStack.Push(Instantiate(textPrefab));
        }
    }

    public void CreateSprayObject(string list, string itemName)
    {
        sprayObjectDB[list][itemName].Create();
    }

    public void PushSIO(SprayImageObject sio)
    {
        if (imageStack.Count < imageObjectCount)
        {
            imageStack.Push(sio);  
        }
    }
    public SprayImageObject PopSIO()
    {
        if (imageStack.Count > 0)
        {
            return imageStack.Pop();
        }
        return null;
    }

    public void PushSTO(SprayTextObject sto)
    {
        if (textStack.Count < textObjectCount)
        {
            textStack.Push(sto);
        }
    }
    public SprayTextObject PopSTO()
    {
        if(textStack.Count > 0)
        {
            return textStack.Pop();
        }
        return null;
    }
}
