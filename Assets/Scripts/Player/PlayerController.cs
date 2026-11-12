using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
   
    
    public GameObject gunMuzzle;
    [Range (0.3f,1f)] public float muzzlePosOffset = 0.5f;
    [Range(0, 50)] public  float moveSpeed = 5.0f;
 
  
    public  HPController hpControllerPrefab; 
    
    private HPController hpController =null;

    [SerializeField] private WeaponController weaponController;
    
    
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 muzzlePosition = Vector3.zero;
    private int switchWeaponHandledFrame = -1;
    private bool fireHeld;
    private float nextFireTime;
 
    private void Awake()
    {  

         if (gunMuzzle == null)
         {
             Debug.LogError("PlayerController :: GunMuzzle is null  !!!!!!!!!!  ");
         }

         if (hpControllerPrefab == null)
         {
             Debug.LogError("PlayerController :: hpControllerPrefab is null  !!!!!!!!!!!  ");

             return;
         }

         if (weaponController == null)
         {
             Debug.LogError("PlayerController :: WeaponController is null");
             
         }

         if (hpController == null)
         {
             hpController = Instantiate(hpControllerPrefab ,transform);
         }
         
         
         
         hpController.SetMaxHP(100);
    }

    private void Update()
    { 
        // Keep the R-key shortcut responsive even if the PlayerInput UnityEvent is not bound.
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            SwitchWeaponOncePerFrame();
        }

        if(hpController.IsDead)
        {
            return;
        }

        transform.position += moveDirection * moveSpeed * Time.deltaTime; 
        
        gunMuzzle.transform.position = transform.position  +(muzzlePosition*muzzlePosOffset);

        if (fireHeld &&
            weaponController != null &&
            weaponController.currentWeaponType == WeaponType.Laser &&
            Time.time >= nextFireTime)
        {
            weaponController.Fire(transform.position, muzzlePosition);
            nextFireTime = Time.time + 1f / weaponController.CurrentWeaponFireRate;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.started) return;
        
        moveDirection = context.ReadValue<Vector2>();

        if(moveDirection != Vector3.zero) muzzlePosition = moveDirection;
    }

    public void OnSwitchWeapon(InputAction.CallbackContext context) 
    {
        if (!context.performed) return;
        SwitchWeaponOncePerFrame();
    }

    private void SwitchWeaponOncePerFrame()
    {
        if (switchWeaponHandledFrame == Time.frameCount) return;
        switchWeaponHandledFrame = Time.frameCount;

        if (weaponController != null)
        {
            weaponController.SwitchWeapon();
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            fireHeld = false;
            return;
        }

        if (weaponController == null) return;

        if (weaponController.currentWeaponType == WeaponType.Laser)
        {
            if (!fireHeld)
            {
                fireHeld = true;
                nextFireTime = Time.time;
            }

            return;
        }

        fireHeld = false;
        if (context.performed)
        {
            weaponController.Fire(transform.position, muzzlePosition);
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        hpController.TakeDamage(2);

       
    }
}
