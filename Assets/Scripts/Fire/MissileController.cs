using DefaultNamespace;
using UnityEngine;

public class MissileController : MonoBehaviour
{    
    protected float moveSpeed = 10f;
    protected float baseDamage = 10;
    protected bool IsPiercing = false;
 
    public float Damage=>baseDamage;  
    protected bool fired = false;
    public bool flying => fired; // 발사되었냐? 날아가고 있냐?
    public  bool readyToMove =>!fired; // 아직 발사안된거 맞지? 지금 장전되어있지?
    
     
    protected Vector3 moveDirection = Vector3.zero;
    public Vector3 Direction => moveDirection;
 
     
    public virtual void Build(WeaponDefinition def)
    {
        
        moveSpeed = def.Speed;
        baseDamage = def.Damage;
        IsPiercing = def.isPiercing;
        
        
        var spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("MissileController : " + "SpriteRenderer not found");

            return;
        }

        spriteRenderer.color = def.color;
        
        if(def.sprite != null) spriteRenderer.sprite = def.sprite;

    }

    // Update is called once per frame
    protected virtual void Update()
    {
         Move(); 
        if (ScreenWorldBounds.IsOutside(transform.position,3f) == true)
        {
            ReturnToReload();
        }
       
    }

    protected  virtual void Move()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
 
    public virtual void Fire(Vector3 firePosition , Vector3 fireDirection)
    {
        if(flying) return; //미사일 발사되었어?
        
        if(fireDirection == Vector3.zero) return;
        
        gameObject.SetActive(true);
        
        transform.position = firePosition;
        moveDirection = fireDirection;
        
        fired = true;
    }

    public virtual void ReturnToReload()
    {
        gameObject.SetActive(false);
        fired = false;
        
    }
    
    protected virtual void OnCollisionEnter2D(Collision2D other) // 충돌처리....
    {

        if (other.gameObject.CompareTag("Enemy"))
        {
            if(!IsPiercing) ReturnToReload();
        }
        
    }
}
