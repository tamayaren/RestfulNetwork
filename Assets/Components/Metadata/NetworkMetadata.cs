using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    public class NetworkMetadata : MonoBehaviour
    {
        public static readonly string BaseURL = "https://api.restful-api.dev/";
    }

    [Serializable]
    public struct WebObjectData
    {
        public string id;
        public string name;
        public WebObjectDataMetadata? data;
        public string createdAt;
    }
    
    [Serializable]
    public struct PostWebObjectData
    {
        public string id;
        public string name;
        public string createdAt;
        public WebObjectDataMetadata data;
    }

    [Serializable]
    public struct WebObjectDataMetadata
    {
        public int? year;
        public float? price;
        [JsonProperty("CPU model")] [CanBeNull] public string cpuModel;
        [JsonProperty("Hard disk size")] [CanBeNull] public string hardDiskSize;
        [JsonProperty("capacity GB")] [CanBeNull] public string capacityGB;
        [CanBeNull] public string color;
        [JsonProperty("Strap Colour")] [CanBeNull] public string strapColour;
        [JsonProperty("Case Size")] [CanBeNull] public string caseSize;
        [JsonProperty("Generation")] [CanBeNull] public string generation;
        [JsonProperty("Screen size")] public float? screenSize;
        [CanBeNull] public string capacity;
    }
}
