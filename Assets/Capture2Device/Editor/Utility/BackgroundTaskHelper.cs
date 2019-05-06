using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;

public static class BackgroundTaskHelper {
   public static void StartBackgroundTask(IEnumerator update, Action end = null)
   {
       EditorApplication.CallbackFunction closureCallback = null;
 
       closureCallback = () =>
       {
           try
           {
               if (update.MoveNext() == false)
               {
                   if (end != null)
                       end();
                   EditorApplication.update -= closureCallback;
               }
           }
           catch (Exception ex)
           {
               if (end != null)
                   end();
               Debug.LogException(ex);
               EditorApplication.update -= closureCallback;
           }
       };
 
       EditorApplication.update += closureCallback;
   }
}
