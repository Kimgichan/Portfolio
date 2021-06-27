using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BookUrl : MonoBehaviour
{
    [SerializeField] BookSelectedAgent bAgent;
    [SerializeField] TextMeshProUGUI title;
    string book_name;
    string url;
    public string URL => url;

    public void Init(string name, string url)
    {
        title.text = $"{name}.txt";
        this.book_name = name;
        this.url = url;
    }

    public void OnPress()
    {
        bAgent.SelectPressed(title.text);
    }
    public void OnRelease()
    {
        bAgent.SafeTimer(book_name, url);
    }
}
