using System;
using System.Collections;
using Networking;
using TMPro;
using UnityEngine;

public class BaseUICore : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject objects;

    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private GameObject objectContainer;

    [SerializeField] private TextMeshProUGUI loadedText;

    [SerializeField] private TextMeshProUGUI updateText;
    [SerializeField] private GameObject updateContainer;

    [SerializeField] private EditPanel panel;
    
    private float t;
    [SerializeField] private NetworkingCore networkingCore;

    private void Start() => this.panel = GetComponent<EditPanel>();
    
    public IEnumerator UpdateText(string text)
    {
        float tD = Time.time;
        this.t = tD;
        
        this.updateText.text = text.ToUpper();
        this.updateContainer.SetActive(true);
        
        yield return new WaitForSeconds(5f);
        if (Mathf.Approximately(this.t, tD))
            this.updateContainer.SetActive(false);
    }   
    
    private void ClearAllChildren(GameObject parent)
    {
        for (int i = parent.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;

            Destroy(child); 
        }
    }
    
    private void Populate()
    {
        ClearAllChildren(this.objectContainer);
        WebObjectData[] webData = NetworkObjectHandler.GetCurrentData();

        for (int i = 0; i < webData.Length; i++)
        {
            WebObjectData data = webData[i];
            if (ReferenceEquals(data, null)) continue;
            
            GameObject dataObject = Instantiate(this.objectPrefab, this.objectContainer.transform);
            ObjectVisualizer visualizer = dataObject.GetComponent<ObjectVisualizer>();
            
            visualizer.Initialize(data, () => this.panel.StartPanel(EditPanel.EditMode.Update, data));
        }
        
        this.loadedText.text = webData.Length.ToString();
        this.objects.SetActive(true);
        
        StartCoroutine(UpdateText($"DATA LOADED: {webData.Length.ToString()}"));
    }
    
    public void PanelLoad()
    {
        StartCoroutine(UpdateText("LOADING DATA.."));
        NetworkingCore.HandleRequest<WebObjectData[]>( "objects", NetworkingCore.WebRequestType.GET, 
        datas =>
        {
            WebObjectData[] data = NetworkObjectHandler.CleanseData(datas);
            
            NetworkObjectHandler.Cache(data);
            NetworkObjectHandler.SetCurrentData(data);
            
            this.menu.SetActive(false);
            Populate();
        }, e =>
        {
            StartCoroutine(UpdateText(e));
        });
    }
}
