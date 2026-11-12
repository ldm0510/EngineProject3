using System.Collections.Generic;
using UnityEngine;
using DefaultNamespace;

public class MissileController_Lazer : MissileController
{
    [Header("Laser Settings")]
    public float beamLength = 16f;
    public float beamThickness = 0.6f;
    [Range(0.05f, 0.9f)] public float minimumPulseScale = 0.2f;
    public float duration = 0.1f;

    private float elapsedTime = 0f;
    private Vector3 originPosition;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D hitbox;
    private readonly HashSet<EnemyController> hitEnemies = new();
    private Color initialColor;
    private Vector3 originalScale;
    private float pulseRate = 10f;

    void Awake()
    {
        originalScale = transform.localScale;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        hitbox = GetComponent<BoxCollider2D>();
        if (hitbox != null) hitbox.enabled = false;
    }

    public override void Build(WeaponDefinition def)
    {
        base.Build(def);
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (hitbox == null) hitbox = GetComponent<BoxCollider2D>();
        if (hitbox != null) hitbox.enabled = false;
        if (spriteRenderer != null) initialColor = spriteRenderer.color;
        pulseRate = Mathf.Max(0.1f, def.fireRate);
    }

    public override void Fire(Vector3 firePosition, Vector3 fireDirection)
    {
        base.Fire(firePosition, fireDirection);
        if (!flying) return;

        elapsedTime = 0f;
        hitEnemies.Clear();
        originPosition = firePosition;
        moveDirection = fireDirection.normalized;
        IsPiercing = true;

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (spriteRenderer != null) spriteRenderer.color = initialColor;
        UpdateLaserTransform(0f);
        HitEnemiesAlongBeam();
    }

    protected override void Move()
    {
        elapsedTime += Time.deltaTime;
        UpdateLaserTransform(elapsedTime);

        if (elapsedTime >= duration) ReturnToReload();
    }

    protected override void Update()
    {
        // The beam's transform is placed at its midpoint, which can be outside
        // the camera even while its firing origin is on screen. Let duration
        // control its lifetime instead of the base projectile bounds check.
        Move();
    }

    private void UpdateLaserTransform(float elapsed)
    {
        float pulse = Mathf.PingPong(elapsed * pulseRate * 2f, 1f);
        float pulseScale = Mathf.Lerp(minimumPulseScale, 1f, pulse);
        float currentLength = beamLength * pulseScale;
        float currentThickness = beamThickness * pulseScale;

        transform.localScale = new Vector3(currentLength, currentThickness, 1f);
        transform.position = originPosition + (moveDirection * (currentLength * 0.5f));

        if (spriteRenderer != null)
        {
            Color c = initialColor;
            c.a = Mathf.Lerp(0.5f, 1f, pulse);
            spriteRenderer.color = c;
        }
    }

    private void HitEnemiesAlongBeam()
    {
        if (beamLength <= 0f) return;

        RaycastHit2D[] hits = Physics2D.RaycastAll(originPosition, moveDirection, beamLength);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;

            EnemyController enemy = hit.collider.GetComponentInParent<EnemyController>();
            if (enemy == null || !enemy.gameObject.activeInHierarchy || !hitEnemies.Add(enemy)) continue;

            enemy.TakeProjectileHit(this);
        }
    }

    public override void ReturnToReload()
    {
        transform.localScale = originalScale;
        if (spriteRenderer != null) spriteRenderer.color = initialColor;
        base.ReturnToReload();
    }
}
