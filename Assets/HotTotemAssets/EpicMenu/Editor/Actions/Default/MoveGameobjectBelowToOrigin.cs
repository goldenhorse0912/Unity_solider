using System.Collections;
using System.Collections.Generic;
using HotTotemAssets.EpicMenu;
using UnityEditor;
using UnityEngine;
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT REMOVE THE FOLLOWING COMMENT
//DISPLAYNAME:Move GameObject below to Origin
//DO NOT REMOVE THE PREVIOUS COMMENT
namespace HotTotemAssets.EpicMenu
{
    public class MoveGameobjectBelowToOrigin : EpicMenuAction
    {
        //Use this Action to write your Custom Code inside
        //The passed argument is the center point of where the
        //EpicMenu pops up (the Mouseposition at the moment of pressing the shortcut)
        //You can use this to make interactions e.g. with the underlying SceneView
        public override void Action(Ray sceneViewToEpicMenuCenterRay)
        {
            RaycastHit hitInfo;
            var hasHit = Physics.Raycast(sceneViewToEpicMenuCenterRay, out hitInfo);
            if (hasHit)
            {
                var hitObj = hitInfo.collider.gameObject;
                hitObj.transform.position = new Vector3(0, 0, 0);
            }
        }
    }
}