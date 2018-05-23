using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace OrcaAssist {

    // Capture to Slack plugin setting singleton
    public class Capture2SlackSetting : ScriptableObject {

        [Header("Editor Setting")]
        public string backupPath;

        [Header("Slack Setting")]
        public string slackToken;
        public string title;
        public string channelName;
        public string comment;

        [MenuItem("Assets/OrcaAssist/Set C2S Backup Path", false, 1101)]
        private static void SetBackupPath() {
            string selectionPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            string fullPath = Application.dataPath + "/" + selectionPath.Replace("Assets/", "");
            InstanceC2S.backupPath = fullPath;
        }

        [MenuItem("Assets/OrcaAssist/Set C2S Backup Path", true)]
        private static bool CheckMenu() {

            if(Selection.objects.Length > 1) {
                return false;
            }

            string selectionPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if(selectionPath.Length == 0) {
                return false;
            }
            
            string fullPath = Application.dataPath + "/" + selectionPath.Replace("Assets/", "");
            FileAttributes file_attr = File.GetAttributes(fullPath);
            if((file_attr & FileAttributes.Directory) != FileAttributes.Directory) {
                return false;
            }

            return true;
        }


        // private string tellegramToken;

        // Singleton Pattern
        private static Capture2SlackSetting _instance = null;
        public static Capture2SlackSetting InstanceC2S {
            get {
                if(!_instance) {
                    _instance = EditorGUIUtility.Load("C2S Setting.asset") as Capture2SlackSetting;
                }
                return _instance;
            }
        }
    }

}