using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class MissilePool : MonoBehaviour
{  
    public List<MissileController> missiles = new();
     
    public void Build(WeaponDefinition def , int maxCount)
    {   
        for (int i = 0; i < maxCount; i++)
        {

            if (def.missilePrefab == null)
            {
                Debug.LogError("MissilePool  WeaponDefinition missilePrefab is null");
                return;
            } 
                
            var m  = Instantiate(def.missilePrefab,transform);
            m.Build(def); // 미사일 데이터로 세팅
            m.gameObject.SetActive(false); 
            missiles.Add(m);
        }
    }

    public void FireMissile(Vector3 startPos, Vector3 direction)
    {   
      foreach (var m in missiles)
      {
          if(m.flying)  continue;
          m.gameObject.SetActive(true);
          m.Fire(startPos, direction);
          break;
      } 
    }
    
}
