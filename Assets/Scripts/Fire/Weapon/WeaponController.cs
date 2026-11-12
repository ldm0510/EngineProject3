using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public enum WeaponType
    {
        Normal = 0,
        Piercing = 1,
        Homing = 2,
        Shockwave = 3,
        Laser = 4,
        Shotgun = 5
    }

    public class WeaponController : MonoBehaviour
    {
        [Header("Settings")]
        public List<WeaponDefinition> weaponList = new();
        private List<WeaponType> weaponTypeList = new();

        readonly Dictionary<WeaponType, MissilePool> weapons = new();

        public int currentWeaponIndex = 0;
        public WeaponType currentWeaponType =>
            weaponTypeList.Count > 0 && currentWeaponIndex >= 0 && currentWeaponIndex < weaponTypeList.Count
                ? weaponTypeList[currentWeaponIndex]
                : default;
        public WeaponDefinition CurrentWeaponDefinition =>
            weaponList == null ? null : weaponList.Find(w => w != null && w.weaponType == currentWeaponType);
        public float CurrentWeaponFireRate =>
            CurrentWeaponDefinition != null ? Mathf.Max(0.1f, CurrentWeaponDefinition.fireRate) : 10f;
        public MissilePool MissilePoolPrefab = null;

        void Awake()
        {
            if (weaponList == null || weaponList.Count == 0)
            {
                Debug.LogError("WeaponController" + "weaponList is empty");
                return;
            }

            if (MissilePoolPrefab == null)
            {
                Debug.LogError("WeaponController" + "MissilePoolPrefab is null");
            }

            BuildWeapons();
        }

        void BuildWeapons()
        {
            weaponTypeList.Clear();
            weapons.Clear();

            foreach (var w in weaponList)
            {
                if (w == null)
                {
                    Debug.LogError("WeaponController has a missing WeaponDefinition reference", this);
                    continue;
                }

                if (!weapons.ContainsKey(w.weaponType))
                {
                    MissilePool pool = Instantiate(MissilePoolPrefab, transform);

                    pool.name = "MissilePool_" + w.weaponType;
                    pool.Build(w, Mathf.Max(1, w.poolSize));
                    weapons.Add(w.weaponType, pool);
                    weaponTypeList.Add(w.weaponType);
                }
            }

            if (weaponTypeList.Count > 0)
            {
                currentWeaponIndex = Mathf.Clamp(currentWeaponIndex, 0, weaponTypeList.Count - 1);
            }
        }

        MissilePool GetWeapon(WeaponType type)
        {
            weapons.TryGetValue(type, out MissilePool pool);
            return pool;
        }

        public void Fire(Vector3 startPos, Vector3 direction)
        {
            if (weaponTypeList.Count == 0) return;

            var weaponPool = GetWeapon(currentWeaponType);

            if (weaponPool == null) return;

            WeaponDefinition currentFire = weaponList.Find(w => w.weaponType == currentWeaponType);

            if (currentFire != null && currentFire.firePattern != null)
            {
                currentFire.firePattern.Fire(weaponPool, startPos, direction);
            }
            else
            {
                weaponPool.FireMissile(startPos, direction);
            }
        }

        public void SwitchWeapon()
        {
            if (weaponTypeList.Count < 2) return;

            currentWeaponIndex++;
            if (currentWeaponIndex >= weaponTypeList.Count) currentWeaponIndex = 0;
        }
    }
}
