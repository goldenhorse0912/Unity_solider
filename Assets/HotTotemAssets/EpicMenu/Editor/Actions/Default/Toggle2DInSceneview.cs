using System.Collections;
using System.Collections.Generic;
using HotTotemAssets.EpicMenu;
using UnityEditor;
using UnityEngine;
//This is an empty ActionTemplate, you can use this 
//for creating custom actions for EpicMenu
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT REMOVE THE FOLLOWING COMMENT
//DISPLAYNAME:Toggle 2D in SceneView
//DO NOT REMOVE THE PREVIOUS COMMENT
namespace HotTotemAssets.EpicMenu
{
    public class Toggle2DInSceneview : EpicMenuAction
    {
        public override void Action(Ray sceneViewToEpicMenuCenterRay)
        {
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.in2DMode = !SceneView.lastActiveSceneView.in2DMode;
            }
        }
    }
}