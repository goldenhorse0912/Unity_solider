using System.Collections;
using System.Collections.Generic;
using HotTotemAssets.EpicMenu;
using UnityEngine;
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT REMOVE THE FOLLOWING COMMENT
//DISPLAYNAME:Create New Empty
//DO NOT REMOVE THE PREVIOUS COMMENT
namespace HotTotemAssets.EpicMenu
{
    public class CreateNewEmpty : EpicMenuAction
    {
        public override void Action(Ray sceneViewToEpicMenuCenterRay)
        {
            var empty = new GameObject("GameObject");
            empty.transform.position = new Vector3(0, 0, 0);
        }
    }
}
