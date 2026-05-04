using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Combat Settings")]
    public int damageAmount = 25;
    public float attackRate = 0.5f;
    private bool isAttacking = false;


    [Header("References")]
    public Animator weaponAnimator;
    public GameObject hitbox;

    public void Awake()
    {
        ResetWeapon();
        hitbox.SetActive(false);
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        weaponAnimator.SetTrigger("Swing");
        
        // PLAY MELEE SOUND
        SoundManager.PlaySound(SoundType.Weapon_Melee, 0.5f);

        yield return new WaitForSeconds(0.2f);

        hitbox.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        hitbox.SetActive(false);

        yield return new WaitForSeconds(attackRate);
        isAttacking = false;
    }

    public void ProcessHit(Collider other)
    {
        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount);
            // PLAY ENEMY HURT SOUND
            SoundManager.PlaySound(SoundType.Enemy_Hurt_Melee);
            hitbox.SetActive(false);
        }
    }
    public void ResetWeapon()
    {
        isAttacking = false; // Reset bool guard
        StopAllCoroutines(); // Kill any active attack timing

        if (weaponAnimator != null)
        {
            weaponAnimator.Play("Idle"); // Force animator back to start
        }

        // Reset physical position if it gets stuck
        transform.localRotation = Quaternion.identity;
    }

    private void OnDisable()
    {
        ResetWeapon();
    }
}