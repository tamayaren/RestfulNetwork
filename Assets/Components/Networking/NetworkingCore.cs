using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEditor.PackageManager.Requests;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    public class NetworkingCore : MonoBehaviour
    {
        [ContextMenu("Get Network")]
        private static async void Get<T>(string route, Action<T> OnSuccess, Action<string> OnError)
        {
            try
            {
                UnityWebRequest request = UnityWebRequest.Get(NetworkMetadata.BaseURL + route);
                request.downloadHandler = new DownloadHandlerBuffer();
                
                UnityWebRequestAsyncOperation operation = request.SendWebRequest();
                while (!operation.isDone) await System.Threading.Tasks.Task.Yield();
                
                switch (request.result)
                {
                    case UnityWebRequest.Result.Success:
                        try
                        {
                            if (request.downloadHandler == null)
                                throw new NullReferenceException();
                            OnSuccess?.Invoke(JsonConvert.DeserializeObject<T>(request.downloadHandler.text));
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                            OnError?.Invoke(e.Message);
                        }

                        break;
                    default:
                        OnError?.Invoke(request.error);
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                OnError?.Invoke(e.StackTrace);
            }
        }

        [ContextMenu("Delete Object")]
        private static async void Delete<T>(string route, Action<T> OnSuccess, Action<string> OnError)
        {
            try
            {
                Debug.Log(route);
                using UnityWebRequest request = UnityWebRequest.Delete(NetworkMetadata.BaseURL + route);
                request.timeout = 10;
                UnityWebRequestAsyncOperation operation = request.SendWebRequest();
                while (!operation.isDone) await System.Threading.Tasks.Task.Yield();
                
                Debug.Log("DELETE OPERATION");
                Debug.Log(request.responseCode);
                switch (request.result)
                {
                    case UnityWebRequest.Result.Success:
                        try
                        {
                            Debug.Log("FINAL SUCCESS");
                            OnSuccess?.Invoke(JsonConvert.DeserializeObject<T>(request.downloadHandler.text));
                        }
                        catch (Exception e)
                        {
                            OnError?.Invoke(e.Message);
                        }

                        break;
                    default:
                        OnError?.Invoke(request.error);
                        break;
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }

        [ContextMenu("Update Object")]
        private static async void Update<T>(string route, WebRequestType type, string body, Action<T> OnSuccess,
            Action<string> OnError)
        {
            try
            {
                if (type != WebRequestType.PUT || type != WebRequestType.POST || type != WebRequestType.PATCH)
                {
                    OnError?.Invoke($"[400] Invalid request type: {type}");
                    return;
                }

                try
                {
                    using UnityWebRequest request =
                        new UnityWebRequest(NetworkMetadata.BaseURL + route, type.ToString());
                    byte[] raw = Encoding.UTF8.GetBytes(body);
                    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(raw);
                    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");
                    await request.SendWebRequest();

                    switch (request.result)
                    {
                        case UnityWebRequest.Result.Success:
                        {
                            try
                            {
                                OnSuccess?.Invoke(JsonConvert.DeserializeObject<T>(request.downloadHandler.text));
                                
                            }
                            catch (Exception e)
                            {
                                OnError?.Invoke(e.Message);
                            }

                            break;
                        }
                        default:
                            OnError?.Invoke(request.error);
                            break;
                    }
                }
                catch (Exception e)
                {
                    OnError?.Invoke(e.Message);
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }

        [ContextMenu("Handle Request")]
        public static void HandleRequest<T>(string route, WebRequestType type, Action<T> OnSuccess,
            Action<string> OnError, string body = null)
        {
            switch (type)
            {
                case WebRequestType.PUT:
                case WebRequestType.PATCH:
                case WebRequestType.POST:
                    Update<T>(route, type, body, OnSuccess, OnError);
                    break;
                case WebRequestType.DELETE:
                    Delete<T>(route, OnSuccess, OnError);
                    break;
                case WebRequestType.GET:
                default:
                    Get<T>(route, OnSuccess, OnError);
                    break;
            }
        }

        public enum WebRequestType
        {
            GET,
            POST,
            PUT,
            PATCH,
            DELETE
        }
    }
}