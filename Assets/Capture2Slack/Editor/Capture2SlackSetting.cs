using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OrcaAssist {

    // Capture to Slack plugin setting singleton
    public class Capture2SlackSetting : ScriptableObject {

        public string title;

        public string backupPath;
        public string slackToken;
        public string defaultChannelName;
        public string defaultComment;

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