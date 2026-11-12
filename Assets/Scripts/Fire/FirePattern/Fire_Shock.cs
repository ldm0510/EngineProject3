using UnityEngine;
using DefaultNamespace;

public class MissileController_Shockwave : MissileController
{
    [Header("Shockwave Settings")]
    public float shockwaveDuration = 0.35f;
    public float maxShockwaveScale = 6f;

    private bool isExploding;
    private float elapsedTime;
    private Vector3 initialScale;
    private SpriteRenderer spriteRenderer;
    private Color initialColor;

    private void Awake()
    {
        initialScale = transform.localScale;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public override void Build(WeaponDefinition def)
    {
        base.Build(def);

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            initialColor = spriteRenderer.color;
        }
    }

    public override void Fire(Vector3 firePosition, Vector3 fireDirection)
    {
        isExploding = false;
        elapsedTime = 0f;
        transform.localScale = initialScale;
        IsPiercing = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = initialColor;
        }

        base.Fire(firePosition, fireDirection);
    }

    protected override void Move()
    {
        if (!isExploding)
        {
            base.Move();
            return;
        }

        elapsedTime += Time.deltaTime;
        float duration = Mathf.Max(shockwaveDuration, 0.01f);
        float t = Mathf.Clamp01(elapsedTime / duration);
        float easeOut = 1f - Mathf.Pow(1f - t, 3f);

        transform.localScale = Vector3.Lerp(initialScale, initialScale * maxShockwaveScale, easeOut);

        if (spriteRenderer != null)
        {
            Color color = initialColor;
            color.a = Mathf.Lerp(0.85f, 0f, t);
            spriteRenderer.color = color;
        }

        if (elapsedTime >= duration)
        {
            ReturnToReload();
        }
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        if (isExploding || !other.gameObject.CompareTag("Enemy")) return;

        transform.position = other.transform.position;
        isExploding = true;
        elapsedTime = 0f;
        IsPiercing = true;
    }

    public override void ReturnToReload()
    {
        isExploding = false;
        elapsedTime = 0f;
        transform.localScale = initialScale;
        IsPiercing = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = initialColor;
        }

        base.ReturnToReload();
    }
}
