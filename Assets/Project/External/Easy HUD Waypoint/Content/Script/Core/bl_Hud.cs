using System;
using UnityEngine;

public class bl_Hud : MonoBehaviour {
    public bl_HudInfo HudInfo;

    /// <summary>
    /// Instantiate a new Hud
    /// add hud to hud manager in start
    /// </summary>
    void Start () {
        /*if (bl_HudManager.instance != null) {
            if (HudInfo.m_Target == null) {
                HudInfo.m_Target = GetComponent<Transform> ();
            }
            if (HudInfo.ShowDynamically) {
                HudInfo.Hide = true;
            }
        } else {
            Debug.LogError ("Need have a Hud Manager in scene");
        }*/
        bl_HudManager.instance.CreateHud (HudInfo);
    }

    void OnDestroy () {
        if (bl_HudManager.Quitting) return;
        bl_HudManager.instance.RemoveHud (HudInfo);
    }

    public void Show () {
        bl_HudManager.instance.HideStateHud (HudInfo, false);
        // if (bl_HudManager.instance != null) { }
    }

    public void Hide () {
        bl_HudManager.instance.HideStateHud (HudInfo, true);
        // if (bl_HudManager.instance != null) { }
    }
}