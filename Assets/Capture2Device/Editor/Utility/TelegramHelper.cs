using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace TelegramHelper {
    
    [Serializable]
    public class UploadData {
        public string Token = string.Empty;
        public string FileName = string.Empty;
        public string ChatId = string.Empty;
        public Texture2D ScreenShot = null;
    }

    public static class TelegramHelper {

        private static string BaseUrl = "https://api.telegram.org/bot";

        public static IEnumerator UploadScreenShot(UploadData data, Action onSuccess = null, Action<string> onError = null) {
            
            yield return new WaitForSeconds(0.1f);
         
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            byte[] contents = data.ScreenShot.EncodeToPNG();
            
            formData.Add(new MultipartFormDataSection("chat_id", data.ChatId));
            formData.Add(new MultipartFormDataSection("caption", data.FileName));
            formData.Add(new MultipartFormFileSection("photo", contents, $"{data.FileName}.png", "image/png"));

            UnityWebRequest www = UnityWebRequest.Post($"{BaseUrl}{data.Token}/sendPhoto", formData);
            
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
