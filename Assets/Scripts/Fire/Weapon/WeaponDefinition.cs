using UnityEngine;
namespace DefaultNamespace
{ 
    [CreateAssetMenu(fileName = "WeaponDefinition", menuName = "CK_Unity/WeaponDefinition")]
    public class WeaponDefinition : ScriptableObject
    {
        public  WeaponType weaponType;
        public Sprite sprite;
        public float Damage;
        public float Speed;
        public float Range;
        public float cooldown;
        [Min(0.1f)] public float fireRate = 10f;
        [Min(1)] public int poolSize = 20;
        public bool isPiercing;
        public Color color;
        
        public MissileController missilePrefab;
        public FirePatternSO firePattern;
    }
}
