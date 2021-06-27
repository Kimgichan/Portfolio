using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Book : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] BookSelectedAgent book_agent;

    public void SetData(string book)
    {
        title.text = book;
    }

    public void OnPresse()
    {
        book_agent.SelectPressed(title.text);
    }
    public void OnRelease()
    {
        book_agent.SafeCall();
    }
}
