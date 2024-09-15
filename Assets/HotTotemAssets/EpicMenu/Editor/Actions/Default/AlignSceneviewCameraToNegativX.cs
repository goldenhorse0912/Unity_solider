using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//This is an empty ActionTemplate, you can use this 
//for creating custom actions for EpicMenu
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT REMOVE THE FOLLOWING COMMENT
//DISPLAYNAME:Align SceneView Camera to -X
//DO NOT REMOVE THE PREVIOUS COMMENT
namespace HotTotemAssets.EpicMenu
{
    public class AlignSceneviewCameraToNegativX : EpicMenuAction
    {
        public override void Action(Ray sceneViewToEpicMenuCenterRay)
        {
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.LookAt(SceneView.lastActiveSceneView.pivot, Quaternion.Euler(0, 90, 0));
            }
        }
    }
}
