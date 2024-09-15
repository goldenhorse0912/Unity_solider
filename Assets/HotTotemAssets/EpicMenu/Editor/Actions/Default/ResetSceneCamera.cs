using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HotTotemAssets.EpicMenu;
using UnityEditor;
using UnityEngine;
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT REMOVE THE FOLLOWING COMMENT
//DISPLAYNAME:Reset Scene Camera
//DO NOT REMOVE THE PREVIOUS COMMENT
namespace HotTotemAssets.EpicMenu
{
    public class ResetSceneCamera : EpicMenuAction
    {
        //Use this Action to write your Custom Code inside
        //The passed argument is the center point of where the
        //EpicMenu pops up (the Mouseposition at the moment of pressing the shortcut)
        //You can use this to make interactions e.g. with the underlying SceneView
        public override void Action(Ray sceneViewToEpicMenuCenterRay)
        {
            if (SceneView.lastActiveSceneView != null)
            {
                MethodInfo info = SceneView.lastActiveSceneView.GetType().
                    GetMethod("OnNewProjectLayoutWasCreated", BindingFlags.Instance | BindingFlags.NonPublic);
                info.Invoke(SceneView.lastActiveSceneView, null);
            }
        }
    }
}
