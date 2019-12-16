using System;
using System.Collections;
using UnityEngine;
using UnityEditor;

using EditorCoroutines;
using SlackHelper;
using TelegramHelper;
using DiscordHelper;

namespace OrcaAssist {

    public class Capture2Device {

        private const string LogTag = "[Capture to Device] {0}";

        private static string _fileName = string.Empty;
        private static string filePath = string.Empty;
        private Texture2D _screenshot = null;

        private static Capture2Device _instance = null;
        public static Capture2Device Instance {
            get { return _instance ?? (_instance = new Capture2Device()); }
        }
        
        public Capture2DeviceSetting SettingData {
            get {
                return Capture2DeviceSetting.InstanceC2D;
            }
        }
        
        [MenuItem("OrcaAssist/Capture 2 Device/to Slack", false, 100)]
        public static void ToSlack() {
            
            EditorCoroutines.EditorCoroutines.StartCoroutine(Instance.CaptureToSlack(), Instance);
        }

        private IEnumerator CaptureToSlack() {
            
            if(!SettingData.IsCompletedSlackData()) yield break;

            _fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            filePath = $"{SettingData.BackupPath}/{_fileName}.png";

            yield return EditorCoroutines.EditorCoroutines.StartCoroutine(CaptureGameView(filePath), this);

            var uploadData = new SlackHelper.UploadData {
                Token = SettingData.SlackToken,
                Title = SettingData.SlackTitle,
                InitialComment = SettingData.SlackComment,
                Filename = _fileName,
                Channels = SettingData.SlackChannelName,
            };

            _screenshot = new Texture2D(1, 1);
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            _screenshot.LoadImage(bytes);
            uploadData.ScreenShot = _screenshot;

            yield return EditorCoroutines.EditorCoroutines.StartCoroutine(SlackHelper.SlackHelper.UploadScreenShot(uploadData, OnSuccess, OnError), this);
        }

        [MenuItem("OrcaAssist/Capture 2 Device/to Telegram", false, 101)]
        public static void ToTelegram() {

            EditorCoroutines.EditorCoroutines.StartCoroutine(Instance.CaptureToTelegram(), Instance);
        }

        private IEnumerator CaptureToTelegram() {
            
            if(!SettingData.IsCompletedTelegaramData()) yield break;

            _fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            filePath = $"{SettingData.BackupPath}/{_fileName}.png";

            yield return EditorCoroutines.EditorCoroutines.StartCoroutine(CaptureGameView(filePath), this);

            var uploadData = new TelegramHelper.UploadData {
                Token = SettingData.TelegramToken,
                FileName = _fileName,
                ChatId = SettingData.TelegramToken,
            };

            _screenshot = new Texture2D(1, 1);
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            _screenshot.LoadImage(bytes);
            uploadData.ScreenShot = _screenshot;

            yield return EditorCoroutines.EditorCoroutines.StartCoroutine(TelegramHelper.TelegramHelper.UploadScreenShot(uploadData, OnSuccess, OnError), this);
        }

        [MenuItem("OrcaAssist/Capture 2 Device/to Discord", false, 102)]
        public static void ToDiscord() {

            EditorCoroutines.EditorCoroutines.StartCoroutine(Instance.CaptureToDiscord(), Instance);
        }

        private IEnumerator CaptureToDiscord() {
            
            if(!SettingData.IsCompletedDiscordData()) yield break;

            _fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            filePath = $"{SettingData.BackupPath}/{_fileName}.png";

            yield return EditorCoroutines.EditorCoroutines.StartCoroutine(CaptureGameView(filePath), this);

            var uploadData = new DiscordHelper.UploadData {
                webHookId = SettingData.DiscordWebhookId,
                Token = SettingData.DiscordToken,
                FileName = _fileName
            };

            _screenshot = new Texture2D(1, 1);
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            _screenshot.LoadImage(bytes);
            uploadData.ScreenShot = _screenshot;

            yield return EditorCoroutines.EditorCoroutines.StartCoroutine(DiscordHelper.DiscordHelper.UploadScreenShot(uploadData, OnSuccess, OnError), this);
        }


        [MenuItem("OrcaAssist/Capture 2 Device/Setting", false, 200)]
        public static void FocusSettingFile() {
            EditorGUIUtility.PingObject(Instance.SettingData);
        }


        private static IEnumerator CaptureGameView(string filePath) {

            // 1. Caputre Game View
#if UNITY_2017_1_OR_NEWER
            ScreenCapture.CaptureScreenshot(filePath);
#else
            Application.CaptureScreenshot(filePath);
#endif
            Debug.Log(string.Format(LogTag, "Export scrennshot at " + filePath));

            // 2. Wait file write complete
            while(true) {
                if(System.IO.File.Exists(filePath)) {
                    break;
                }
                else {
                    yield return new WaitForSeconds(0.5f);
                }
            }

            AssetDatabase.Refresh();

            yield return null;
        }
        
        private static void OnSuccess() {
            Debug.Log(string.Format(LogTag, "Upload Success!! Check your slack"));
        }

        private static void OnError(string e) {
            Debug.LogError(string.Format(LogTag, "Upload FAIL!! Error message is... " + e));
        }
    }
}
