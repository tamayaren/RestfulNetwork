using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ObjectVisualizer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private Button edit;
    public void Initialize(WebObjectData data, UnityAction action)
    {
        this.nameText.text = data.name;
        this.descriptionText.text = $"ID: {data.id}";
        
        this.edit.onClick.AddListener(action);
    }
}
