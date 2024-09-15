using System.Collections;
using Sirenix.OdinInspector;

namespace VIRTUE {
    public static class PooledObjectIds {
        public static IEnumerable PoolIds = new ValueDropdownList<string> {
            {
                "01_Coin", "Coin"
            }, {
                "02_PistolBullet", "PistolBullet"
            }, {
                "03_RifleBullet", "RifleBullet"
            }, {
                "04_SniperBullet", "SniperBullet"
            }, {
                "05_JeepBullet", "JeepBullet"
            }, {
                "06_ShotgunBullet", "ShotgunBullet"
            }, {
                "07_TankMissile", "TankMissile"
            }, {
                "08_HelicopterBullet", "HelicopterBullet"
            },{
                "09_HelicopterMissile", "HelicopterMissile"
            },
        };
    }
}