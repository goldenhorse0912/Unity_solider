using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This is an empty ActionTemplate, you can use this 
//for creating custom actions for EpicMenu
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT REMOVE THE FOLLOWING COMMENT
//DISPLAYNAME:Clear Console
//DO NOT REMOVE THE PREVIOUS COMMENT
namespace HotTotemAssets.EpicMenu
{
    public class ClearConsole : EpicMenuAction
    {
        public override void Action(Ray sceneViewToEpicMenuCenterRay)
        {
            var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }
    }
}
