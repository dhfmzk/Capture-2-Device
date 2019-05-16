using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SlackHelper {
    
    [Serializable]
    public class UploadData {
        public string Token = string.Empty;
        public string Filename = string.Empty;
        public string Title = string.Empty;
        public string InitialComment = string.Empty;
        public string Channels = string.Empty;
        public Texture2D ScreenShot = null;
    }

    public static class SlackHelper {

        private const string apiUrl = "https://slack.com/api";

        public static IEnumerator UploadScreenShot(UploadData data, Action onSuccess = null, Action<string> onError = null) {
            
            yield return new WaitForSeconds(0.1f);

            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            byte[] contents = data.ScreenShot.EncodeToPNG();

            formData.Add(new MultipartFormDataSection("token", data.Token));
            formData.Add(new MultipartFormDataSection("title", data.Title));
            formData.Add(new MultipartFormDataSection("initial_comment", data.InitialComment));
            formData.Add(new MultipartFormDataSection("channels", data.Channels));
            formData.Add(new MultipartFormFileSection("file", contents, data.Filename, "image/png"));
            
            UnityWebRequest www = UnityWebRequest.Post($"{apiUrl}/files.upload", formData);

            yield return www.SendWebRequest();;
            string error = www.error;

            if(!string.IsNullOrEmpty(error)) {
                if(onError != null) {
                    onError(error);
                }
                yield break;
            }

            if(onSuccess != null) {
                onSuccess();
            }
        }
    }
}
