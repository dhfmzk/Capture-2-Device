using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DiscordHelper {
    
    [Serializable]
    public class UploadData {
        public string webHookId = string.Empty;
        public string Token = string.Empty;
        public string FileName = string.Empty;
        public Texture2D ScreenShot = null;
    }

    public static class DiscordHelper {

        private static string apiUrl = "https://discordapp.com/api/webhooks";

        public static IEnumerator UploadScreenShot(UploadData data, Action onSuccess = null, Action<string> onError = null) {
            
            yield return new WaitForSeconds(0.1f);
         
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            byte[] contents = data.ScreenShot.EncodeToPNG();

            formData.Add( new MultipartFormFileSection("file", contents, data.FileName+".png", "image/png") );

            UnityWebRequest www = UnityWebRequest.Post($"{apiUrl}/{data.webHookId}/{data.Token}", formData);

            yield return www.SendWebRequest();
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
