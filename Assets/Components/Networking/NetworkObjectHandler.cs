using System.Collections.Generic;
using JetBrains.Annotations;
using Networking;
using UnityEngine;
using UnityEngine.Events;

public class NetworkObjectHandler : MonoBehaviour
{
    private static Queue<WebObjectData[]> cached = new Queue<WebObjectData[]>();
    private static WebObjectData[] currentData;

    public static UnityEvent<WebObjectData[]> CurrentDataChanged = new UnityEvent<WebObjectData[]>();
    
    public static WebObjectData[] GetCurrentData() => currentData;

    public static void SetCurrentData(WebObjectData[] data)
    {
        currentData = data;
        CurrentDataChanged.Invoke(currentData);
    }
    
    public static void Cache(WebObjectData[] data)
    {
        cached.Enqueue(data);

        if (cached.Count > 10)
            cached.Dequeue();
    }
    
    [CanBeNull]
    public static WebObjectData[] GetCache() => cached.Count > 0 ? cached.Dequeue() : null;

    public static WebObjectData[] CleanseData(WebObjectData[] data)
    {
        WebObjectData[] cleansed = data;

        for (int i = 0; i < cleansed.Length; i++)
        {
            WebObjectData webObject = cleansed[i];

            if (webObject is { data: null })
            {
                cleansed[i].data = new WebObjectDataMetadata();
            }
        }
        
        return cleansed;
    }
}
