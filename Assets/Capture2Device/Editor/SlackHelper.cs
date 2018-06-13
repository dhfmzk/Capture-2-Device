using System;
using System.Collections;
using UnityEngine;

namespace SlackHelper {
    
    [Serializable]
    public class UploadData {
        public string token = string.Empty;
        public string filename = string.Empty;
        public string title = string.Empty;
        public string initial_comment = string.Empty;
        public string channels = string.Empty;
        public Texture2D screenShot = null;
    }

    public static class SlackHelper {
        
        public static IEnumerator UploadScreenShot(UploadData data, Action onSuccess = null, Action<string> onError = null) {
            yield return new WaitForSeconds(0.1f);
         
            WWWForm form = new WWWForm();
            byte[] contents = data.screenShot.EncodeToPNG();

            form.AddField("token", data.token);
            form.AddField("title", data.title);
            form.AddField("initial_comment", data.initial_comment);
            form.AddField("channels", data.channels);

            form.AddBinaryData("file", contents, data.filename, "image/png");

            string url = "https://slack.com/api/files.upload";
            WWW www = new WWW(url, form);
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
