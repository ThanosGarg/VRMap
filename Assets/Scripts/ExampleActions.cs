using UnityEngine;
using System;


public class ExampleActions : MonoBehaviour
{
    public Action onExampleEvent;
    void Start()
    {
        onExampleEvent += ExampleMethod;
        onExampleEvent += Test;
        onExampleEvent.Invoke();

    }
    public void ExampleMethod()
    {
        Debug.Log("ExampleMethod called!");

    }
    public void Test()
    {
        Debug.Log("test");
    }
}
