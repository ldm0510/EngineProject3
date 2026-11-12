using UnityEngine;

namespace DefaultNamespace
{
    public interface IFirePattern
    {
        void Fire(MissilePool pool, Vector3 startPos, Vector3 direction);
    }
}