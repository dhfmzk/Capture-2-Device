﻿using System;
using System.Collections;
using UnityEngine;
using UnityEditor;

using SlackHelper;

namespace OrcaAssist {

    public class Capture2Device {

        private const string LogTag = "[Capture to Device] {0}";

        // ---------------------------------------------------------------------------
        // Data
        // ---------------------------------------------------------------------------
        private string _fileName = string.Empty;
        private UploadData _uploadData = null;
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

        // ---------------------------------------------------------------------------
        // Public Method
        // ---------------------------------------------------------------------------
        [MenuItem("OrcaAssist/Capture 2 Device/to Slack", false, 100)]
        public static void ToSlack() {
            EditorCoroutines.EditorCoroutines.StartCoroutine(Instance.UploadToSlack(), Instance);
        }

        [MenuItem("OrcaAssist/Capture 2 Device/Setting", false, 200)]
        public static void FocusSettingFile() {
            // Focussing Singleton setting
            EditorGUIUtility.PingObject(Instance.SettingData);
        }

        // ---------------------------------------------------------------------------
        // Upload Worker
        // ---------------------------------------------------------------------------
        private IEnumerator UploadToSlack() {

            if(!SettingData.IsCompletedData()) {
                yield break;
            }

            // 0. Get file name
            _fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            string filePath = SettingData.BackupPath + "/" + _fileName + ".png";

            // 1. Start Capture
            yield return EditorCoroutines.EditorCoroutines.StartCoroutine(CaptureGameView(filePath), this);

            // 2. Get image file
            _screenshot = new Texture2D(1, 1);
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);

            _screenshot.LoadImage(bytes);
            _uploadData.ScreenShot = _screenshot;

            // 3. Start Upload
            yield return EditorCoroutines.EditorCoroutines.StartCoroutine(SlackHelper.SlackHelper.UploadScreenShot(_uploadData, OnSuccess, OnError), this);

        }

        private IEnumerator CaptureGameView(string filePath) {

            // 0. Make Upload Data
            _uploadData = new UploadData {
                Token = SettingData.SlackToken,
                Title = SettingData.Title,
                InitialComment = SettingData.Comment,
                Filename = _fileName,
                Channels = SettingData.ChannelName
            };

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

            yield return null;
        }
        
        // ---------------------------------------------------------------------------
        // Callback Functions
        // ---------------------------------------------------------------------------
        private static void OnSuccess() {
            Debug.Log(string.Format(LogTag, "Upload Success!! Check your slack"));
        }

        private static void OnError(string e) {
            Debug.LogError(string.Format(LogTag, "Upload FAIL!! Error message is... " + e));
        }
    }
}
