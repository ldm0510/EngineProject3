using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "ShotgunPattern", menuName = "CK_Unity/FirePattern/Shotgun")]
    public class MissileController_Shotgun : FirePatternSO
    {
        [Header("Shotgun Settings")]
        public int pelletCount = 5;       // 동시 발사 개수
        public float spreadAngle = 45f;   // 퍼지는 각도

        public override void Fire(MissilePool pool, Vector3 startPos, Vector3 direction)
        {
            if (pool == null || direction == Vector3.zero) return;

            direction.Normalize();

            if (pelletCount <= 1)
            {
                pool.FireMissile(startPos, direction);
                return;
            }

            float startAngle = -spreadAngle * 0.5f;
            float angleStep = spreadAngle / (pelletCount - 1);

            for (int i = 0; i < pelletCount; i++)
            {
                float currentAngle = startAngle + (angleStep * i);
                Vector3 rotatedDir = Quaternion.Euler(0f, 0f, currentAngle) * direction;
                pool.FireMissile(startPos, rotatedDir);
            }
        }
    }
}