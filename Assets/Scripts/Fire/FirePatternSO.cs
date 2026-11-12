using UnityEngine;

namespace DefaultNamespace
{
    public abstract class FirePatternSO : ScriptableObject, IFirePattern
    {
        public abstract void Fire(MissilePool pool, Vector3 startPos, Vector3 direction);
    }
}