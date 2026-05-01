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
        isAttacking = false;
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
        SoundManager.PlaySound(SoundType.Weapon_Melee);

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
            hitbox.SetActive(false);
        }
    }
    public void ResetWeapon()
    {
        isAttacking = false;
        StopAllCoroutines();
        if (weaponAnimator != null)
        {
            weaponAnimator.Play("Idle");
        }
    }
    private void OnDisable()
    {
        ResetWeapon();
    }
}