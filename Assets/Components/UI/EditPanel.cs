using System;
using Networking;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditPanel : MonoBehaviour
{
    [SerializeField] private GameObject editPanel;

    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField idInput;
    [SerializeField] private TMP_InputField dataJson;

    [SerializeField] private GameObject deleteButton;
    [SerializeField] private GameObject createButton;
    [SerializeField] private GameObject cancelButton;

    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private Button updateButton;
    [SerializeField] private BaseUICore core;
    private WebObjectData? currentWebData;
    public void StartPanel(EditMode mode, WebObjectData? data)
    {
        this.editPanel.SetActive(true);
        
        this.deleteButton.SetActive(false);
        this.createButton.SetActive(false);

        switch (mode)
        {
            case EditMode.Add:
                this.createButton.SetActive(true);
                this.idInput.text = "---";

                this.text.text = "ADD NEW DATA";
                
                this.updateButton.onClick.AddListener(() =>
                {
                    NetworkingCore.HandleRequest<WebObjectData>("objects", NetworkingCore.WebRequestType.POST, 
                        objectData =>
                        {
                            CancelPanel();
                            this.core.PanelLoad();
                        }, e =>  
                        this.core.UpdateText(e), JsonConvert.SerializeObject(data));
                    
                    this.updateButton.onClick.RemoveAllListeners();
                });
                break;
            
            case EditMode.Update:
                this.deleteButton.SetActive(true);
                this.createButton.SetActive(true);
                if (data != null)
                {
                    WebObjectData aData = (WebObjectData)data;
                    this.text.text = $"EDIT: {aData.id} - {aData.name}";
                    this.idInput.text = aData.id;
                    this.nameInput.text = aData.name;

                    if (aData.data != null)
                        this.dataJson.text = JsonUtility.ToJson(aData.data, true);
                    
                    this.updateButton.onClick.AddListener(() =>
                    {
                        WebObjectData newData = new WebObjectData();
                        newData.name = this.nameInput.text;
                        try
                        {
                            newData.data = JsonConvert.DeserializeObject<WebObjectDataMetadata>(this.dataJson.text);
                        }
                        catch (Exception e)
                        {
                            newData.data = new WebObjectDataMetadata();
                        }
                        
                        NetworkingCore.HandleRequest<WebObjectData>($"objects{aData.id}", NetworkingCore.WebRequestType.POST, 
                            objectData =>
                            {
                                CancelPanel();
                                this.core.PanelLoad();
                            }, e =>  
                                this.core.UpdateText(e), JsonConvert.SerializeObject(newData));
                    
                        this.updateButton.onClick.RemoveAllListeners();
                    });
                    
                    this.currentWebData = aData;
                }
                break;
        }
    }

    public void CancelPanel()
    {
        this.editPanel.SetActive(false);
        this.currentWebData = null;
    }

    public void DeleteObject()
    {
        if (this.currentWebData != null)
        {
            WebObjectData data = (WebObjectData)this.currentWebData;
            NetworkingCore.HandleRequest<WebObjectData>($"objects/{data.id}", NetworkingCore.WebRequestType.DELETE,
                (data) =>
                {
                    this.core.PanelLoad();
                    CancelPanel();
                }, (e) =>
                {
                    this.core.UpdateText(e);
                });
        }
    }
    
    public enum EditMode
    {
        Add,
        Update,
    }
}
