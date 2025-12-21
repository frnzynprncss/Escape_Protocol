using UnityEngine;

[System.Serializable]
public class PlayerSpriteSet
{
    public Sprite[] walkFront = new Sprite[6];
    public Sprite[] walkBack = new Sprite[6];
    public Sprite[] walkLeft = new Sprite[8];
    public Sprite[] walkRight = new Sprite[8];

    // New Idle Sprite Arrays
    public Sprite[] idleFront = new Sprite[3];
    public Sprite[] idleBack = new Sprite[3];
    public Sprite[] idleLeft = new Sprite[3];
    public Sprite[] idleRight = new Sprite[3];
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Control Layout")]
    public PlayerControl controls;
    public GameObject weapon_pivot;
    public float moveSpeed = 5f;

    [Header("Sprite Sets")]
    public PlayerSpriteSet pistolSprites;
    public PlayerSpriteSet shotgunSprites;
    public PlayerSpriteSet rifleSprites;

    private PlayerSpriteSet activeSprites;

    [Header("Animation Settings")]
    public float animationSpeedFPS = 12f;

    private Rigidbody2D rb;
    private Vector2 input;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastDirection = Vector2.down;

    private float frameTimer;
    private int currentFrameIndex;
    private Sprite[] lastAnimation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("PlayerMovement requires a SpriteRenderer component on this GameObject OR one of its children to display sprites.");
        }

        activeSprites = pistolSprites; // default weapon
    }

    private void Update()
    {
        input = get_input();
        rotate_weapon();
        animate_player();
    }

    private void FixedUpdate()
    {
        rb.velocity = input * moveSpeed * Time.deltaTime;
    }

    private void rotate_weapon()
    {
        if (input == Vector2.zero) return;

        float angle_radians = Mathf.Atan2(input.y, input.x);
        float angle_degrees = angle_radians * Mathf.Rad2Deg;

        Quaternion weapon_rotation = Quaternion.Euler(0f, 0f, angle_degrees);
        weapon_pivot.transform.rotation = weapon_rotation;
    }

    private Vector2 get_input()
    {
        float x_input = 0f;
        float y_input = 0f;

        if (Input.GetKey(controls.move_right)) x_input = 1f;
        if (Input.GetKey(controls.move_left)) x_input = -1f;
        if (Input.GetKey(controls.move_up)) y_input = 1f;
        if (Input.GetKey(controls.move_down)) y_input = -1f;

        return new Vector2(x_input, y_input).normalized;
    }

    private void animate_player()
    {
        if (spriteRenderer == null) return;

        Sprite[] currentAnimation = null;

        if (input != Vector2.zero)
        {
            // MOVEMENT ANIMATION
            lastDirection = input;

            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                currentAnimation = (input.x > 0) ? activeSprites.walkRight : activeSprites.walkLeft;
            }
            else
            {
                currentAnimation = (input.y > 0) ? activeSprites.walkBack : activeSprites.walkFront;
            }
        }
        else
        {
            // IDLE ANIMATION
            if (Mathf.Abs(lastDirection.x) > Mathf.Abs(lastDirection.y))
            {
                currentAnimation = (lastDirection.x > 0) ? activeSprites.idleRight : activeSprites.idleLeft;
            }
            else
            {
                currentAnimation = (lastDirection.y > 0) ? activeSprites.idleBack : activeSprites.idleFront;
            }
        }

        // Logic to play the chosen animation (Walking or Idle)
        if (currentAnimation == null || currentAnimation.Length == 0)
            return;

        if (currentAnimation != lastAnimation)
        {
            currentFrameIndex = 0;
            frameTimer = 0f;
            lastAnimation = currentAnimation;
        }

        float frameTime = 1f / animationSpeedFPS;
        frameTimer -= Time.deltaTime;

        if (frameTimer <= 0f)
        {
            currentFrameIndex++;
            if (currentFrameIndex >= currentAnimation.Length)
                currentFrameIndex = 0;

            frameTimer = frameTime;
        }

        spriteRenderer.sprite = currentAnimation[currentFrameIndex];
    }

    public enum WeaponType
    {
        Pistol,
        Shotgun,
        Rifle
    }

    public void SetWeaponSprites(WeaponType weapon)
    {
        switch (weapon)
        {
            case WeaponType.Pistol:
                activeSprites = pistolSprites;
                break;
            case WeaponType.Shotgun:
                activeSprites = shotgunSprites;
                break;
            case WeaponType.Rifle:
                activeSprites = rifleSprites;
                break;
        }

        // Reset animation safely
        currentFrameIndex = 0;
        frameTimer = 0f;
        lastAnimation = null;
    }
}