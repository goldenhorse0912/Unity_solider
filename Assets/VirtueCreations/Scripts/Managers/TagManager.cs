using UnityEngine;

namespace VIRTUE {
    public static class PlayerPrefsKey {
        internal const string HAPTICS = "Haptics";
        internal const string AUDIO = "Audio";
        internal const string LEVEL = "Level";
        internal const string BATTLE_SCENE = "BattleScene";
        internal const string FIRST_TIME_LAUNCH = "FirstTimeLaunch";
        internal const string BLESSINGS = "Blessings";
        internal const string FIRST_TIME_BLESSING = "FirstTimeBlessing";
        internal const string RESOURCE_CONFIGS = "ResourceConfigs";
        internal const string UNLOCKED_RESOURCES = "UnlockedResources";
        internal const string TUTORIALCOMPLETED = "TutorialCompleted";
        internal const string TROOPBUILDINGUPCOMPLETE = "TroopBuildingUpComplete";
    }

    public static class Tags {
        internal const string UNTAGGED = "Untagged";
        internal const string PLAYER = "Player";
        internal const string DEAD = "Dead";
        internal const string GENERATINGENERGY = "GeneratingEnergy";
    }

    public static class BuildingTags {
        internal const string MainBase = "MainBase";
        internal const string GroundTroops = "GroundTroops";
        internal const string VehicleTroops = "VehicleTroops";
        internal const string FlyingTroops = "FlyingTroops";
    }

    public static class FileExtensions {
        internal const string JSON = ".json";
        internal const string TXT = ".txt";
        internal const string XML = ".xml";
        internal const string DAT = ".dat";
    }

    public static class AnimatorParams {
        internal static readonly int Activate = Animator.StringToHash ("Activate");
        internal static readonly int Open = Animator.StringToHash ("Open");
        internal static readonly int Move = Animator.StringToHash ("Move");
        internal static readonly int Wave = Animator.StringToHash ("Wave");
        internal static readonly int Attack = Animator.StringToHash ("Attack");
        internal static readonly int Dead = Animator.StringToHash ("Dead");
        internal static readonly int Victory = Animator.StringToHash ("Victory");
        internal static readonly int IsMoving = Animator.StringToHash ("IsMoving");
        internal static readonly int IsMining = Animator.StringToHash ("IsMining");
        internal static readonly int IsChopping = Animator.StringToHash ("IsChopping");
        internal static readonly int IsAttacking = Animator.StringToHash ("IsAttacking");
        internal static readonly int Shoot = Animator.StringToHash ("Shoot");
    }

    public static class ShaderParams {
        public static readonly int GrayscaleAmount = Shader.PropertyToID ("_GrayscaleAmount");
        public static readonly int AlbedoMap = Shader.PropertyToID ("_AlbedoMap");
        public static readonly int AlbedoColor = Shader.PropertyToID ("_AlbedoColor");
        public static readonly int BaseMap = Shader.PropertyToID ("_BaseMap");
        public static readonly int BaseColor = Shader.PropertyToID ("_BaseColor");
        public static readonly int MainTex = Shader.PropertyToID ("_MainTex");
        public static readonly int Color = Shader.PropertyToID ("_Color");
        public static readonly int SColor = Shader.PropertyToID ("_SColor");
        public static readonly int UseMobileMode = Shader.PropertyToID ("_UseMobileMode");
        public static readonly int GoochDarkColor = Shader.PropertyToID ("_GoochDarkColor");
        public static readonly int Specular = Shader.PropertyToID ("_Specular");
    }
}