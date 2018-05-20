using System;
using System.Collections;
using UnityEngine;

using SlackHelper;

namespace OrcaAssist {

    public class Capture2Slack {

        // ---------------------------------------------------------------------------
        // Data
        // ---------------------------------------------------------------------------
        private string fileName = string.Empty;
        private string backupDirPath = string.Empty;
        private string backupPath = string.Empty;
        private UploadData uploadData = null;

        private Texture2D screenshot = null;

        private Capture2SlackSetting _SettingData;
        public Capture2SlackSetting SettingData {
            get {
                return Capture2SlackSetting.InstanceC2S;
            }
        }

        // ---------------------------------------------------------------------------
        // Public Method
        // ---------------------------------------------------------------------------
        public void CaptureAndUpload() {
            EditorCoroutines.EditorCoroutines.StartCoroutine(CaptureAndUploadCoroutine(), this);
        }

        // ---------------------------------------------------------------------------
        // Upload Worker
        // ---------------------------------------------------------------------------
        private IEnumerator CaptureAndUploadCoroutine() {

            this.fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            this.backupPath = backupDirPath + "/" + fileName + ".png";

            uploadData = new UploadData {
                token = SettingData.slackToken,
                title = SettingData.title,
                initial_comment = SettingData.defaultComment,
                filename = fileName,
                channels = SettingData.defaultChannelName
            };

#if UNITY_2017_1_OR_NEWER
            ScreenCapture.CaptureScreenshot(backupPath);
#else
            Application.CaptureScreenshot(backupPath);
#endif
            Debug.Log("[Capture to Slack] Export scrennshot at " + backupPath);

            while(true) {
                if(System.IO.File.Exists(backupPath)) {
                    break;
                }
                else {
                    yield return new WaitForSeconds(0.5f);
                }
            }

            screenshot = new Texture2D(1, 1);
            byte[] bytes;
            bytes = System.IO.File.ReadAllBytes(backupPath);
            screenshot.LoadImage(bytes);

            uploadData.screenShot = screenshot;

            yield return EditorCoroutines.EditorCoroutines.StartCoroutine(SlackAPI.UploadScreenShot(uploadData, this.OnSuccess, this.OnError), this);

        }
        
        void OnSuccess() {
            Debug.Log("[Capture to Slack] Upload Success!! Check your slack");
        }

        void OnError(string _e) {
            Debug.LogError("[Capture to Slack] Upload FAIL!! Error message is... " + _e);
        }
    }
}
