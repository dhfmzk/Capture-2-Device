using System;
using System.Collections;
using UnityEngine;

namespace TelegramHelper {
    
    [Serializable]
    public class UploadData {
        public string Token = string.Empty;
        public string FileName = string.Empty;
        public string ChatId = string.Empty;
        public Texture2D ScreenShot = null;
    }

    public static class TelegramHelper {

        private static string BaseUrl = "https://api.telegram.org/bot{0}/{1}";

        public static IEnumerator UploadScreenShot(UploadData data, Action onSuccess = null, Action<string> onError = null) {
            yield return new WaitForSeconds(0.1f);
         
            WWWForm form = new WWWForm();
            byte[] contents = data.ScreenShot.EncodeToPNG();
            
            form.AddField("chat_id", data.ChatId);
            form.AddField("caption", data.FileName);
            form.AddBinaryData("photo", contents, data.FileName, "image/png");

            WWW www = new WWW(string.Format(BaseUrl, data.Token, "sendPhoto"), form);
            yield return www;
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
